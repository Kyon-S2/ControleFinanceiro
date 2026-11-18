/* =====================================================
   FINANCE CONTROL SYSTEM — app.js
   Auth + Storage: API REST .NET
   ===================================================== */

// ─── 1. CONFIGURAÇÕES E ESTADO GLOBAL ─────────────────
let historico = [];
let historicoGanhos = [];
let categorias = [];
let idRemover = null;
let idRemoverGanho = null;
let idEditar = null;
let idEditarGanho = null;
let limiteMensal = 0;

// ─── 2. AUTENTICAÇÃO E SESSÃO ─────────────────────────
function trocarAba(aba, btn) {
  document.querySelectorAll('.auth-tab').forEach(t => t.classList.remove('active'));
  document.querySelectorAll('.auth-form').forEach(f => f.classList.remove('active'));
  btn.classList.add('active');
  document.getElementById('form-' + aba).classList.add('active');
  limparErrosAuth();
}

function limparErrosAuth() {
  ['err-login-email', 'err-login-senha', 'err-login-geral', 'err-reg-email', 'err-reg-senha', 'err-reg-confirma', 'err-reg-geral']
    .forEach(id => setError(id, ''));
}

async function fazerLogin() {
  const email = document.getElementById('login-email').value.trim();
  const senha = document.getElementById('login-senha').value;

  let valido = true;
  if (!email) { setError('err-login-email', 'Informe seu e-mail.'); valido = false; }
  if (!senha) { setError('err-login-senha', 'Informe sua senha.'); valido = false; }
  if (!valido) return;

  setBloqueadoAuth(true);
  try {
    const dados = await API.post('/api/auth/login', { email, senha });
    API.setToken(dados.token);
    API.setEmail(email);
    await entrarNoApp(email);
  } catch (err) {
    setError('err-login-geral', err.message || 'Erro de conexão.');
  } finally {
    setBloqueadoAuth(false);
  }
}

async function fazerCadastro() {
  const email = document.getElementById('reg-email').value.trim();
  const senha = document.getElementById('reg-senha').value;
  const confirma = document.getElementById('reg-confirma').value;

  let valido = true;
  if (!email) { setError('err-reg-email', 'Informe um e-mail.'); valido = false; }
  if (senha.length < 8) { setError('err-reg-senha', 'Mínimo 8 caracteres.'); valido = false; }
  if (senha !== confirma) { setError('err-reg-confirma', 'As senhas não coincidem.'); valido = false; }
  if (!valido) return;

  setBloqueadoAuth(true);
  try {
    const dados = await API.post('/api/auth/register', { email, senha });
    API.setToken(dados.token);
    API.setEmail(email);
    await entrarNoApp(email);
  } catch (err) {
    setError('err-reg-geral', err.message || 'Erro de conexão.');
  } finally {
    setBloqueadoAuth(false);
  }
}

async function fazerLogout(silencioso = false) {
  API.setToken(null);
  API.setEmail(null);
  historico = [];
  historicoGanhos = [];
  categorias = [];
  idRemover = null;
  idRemoverGanho = null;
  idEditar = null;
  idEditarGanho = null;
  limiteMensal = 0;

  document.getElementById('tela-app').style.display = 'none';
  document.getElementById('tela-auth').style.display = 'flex';
  ['login-email', 'login-senha', 'reg-email', 'reg-senha', 'reg-confirma'].forEach(id => {
    const el = document.getElementById(id);
    if (el) el.value = '';
  });
  limparErrosAuth();
  if (silencioso) {
    showToast('Sessão encerrada.', 'danger');
  }
}

async function entrarNoApp(email) {
  document.getElementById('tela-auth').style.display = 'none';
  document.getElementById('tela-app').style.display = 'block';
  document.getElementById('user-email-display').textContent = email;

  ['stat-total', 'stat-max'].forEach(id => document.getElementById(id).textContent = 'R$ ...');
  document.getElementById('stat-count').textContent = '...';

  await carregarCategorias();
  await carregarLimiteSalvo();

  const resultados = await Promise.allSettled([dbBuscarTodos(), dbBuscarGanhos()]);
  historico       = (resultados[0].status === 'fulfilled' && Array.isArray(resultados[0].value)) ? resultados[0].value : [];
  historicoGanhos = (resultados[1].status === 'fulfilled' && Array.isArray(resultados[1].value)) ? resultados[1].value : [];
  if (resultados.some(r => r.status === 'rejected')) showToast('Aviso: alguns dados não carregaram.', 'danger');
  atualizarStats();
}

// ─── 3. CATEGORIAS ────────────────────────────────────
async function carregarCategorias() {
  try {
    let list = await API.get('/api/categorias');
    if (!list || list.length === 0) {
      const padroes = ['Alimentação', 'Transporte', 'Lazer', 'Saúde', 'Moradia', 'Educação', 'Outros'];
      for (const p of padroes) {
        try { await API.post('/api/categorias', { nome: p }); } catch {}
      }
      list = await API.get('/api/categorias');
    }
    categorias = list || [];
    preencherSelectsCategoria();
  } catch (err) {
    console.error('Erro ao carregar categorias:', err);
  }
}

function preencherSelectsCategoria() {
  const selectIds = ['inp-classe', 'edit-classe', 'ganho-classe', 'edit-ganho-classe', 'filtro-categoria'];
  selectIds.forEach(id => {
    const el = document.getElementById(id);
    if (!el) return;
    const isFiltro = (id === 'filtro-categoria');
    const valorAtual = el.value;
    el.innerHTML = isFiltro
      ? '<option value="">Todas as Categorias</option>'
      : '<option value="" disabled selected>Selecione uma categoria...</option>';

    categorias.forEach(c => {
      const opt = document.createElement('option');
      opt.value = isFiltro ? c.nome : c.id;
      opt.textContent = c.nome;
      el.appendChild(opt);
    });

    if (valorAtual) {
      el.value = valorAtual;
    }
  });
}

// ─── 4. BANCO DE DADOS (API REST) ─────────────────────
function formatarDataExibicao(isoStr) {
  if (!isoStr) return '';
  if (/^\d{2}\/\d{2}\/\d{4}$/.test(isoStr)) return isoStr;
  const parts = isoStr.split('-');
  if (parts.length === 3) {
    const [y, m, d] = parts;
    return `${d.padStart(2, '0')}/${m.padStart(2, '0')}/${y}`;
  }
  return isoStr;
}

function formatarDataIso(dataStr) {
  if (!dataStr) return '';
  if (/^\d{4}-\d{2}-\d{2}$/.test(dataStr)) return dataStr;
  const parts = dataStr.split('/');
  if (parts.length === 3) {
    const [d, m, y] = parts;
    return `${y}-${m.padStart(2, '0')}-${d.padStart(2, '0')}`;
  }
  return dataStr;
}

async function dbBuscarTodos() {
  const list = await API.get('/api/gastos');
  return (list || []).map(g => ({
    id: g.id,
    nome: g.nome,
    valor: g.valor,
    classe: g.categoriaNome || '',
    categoriaId: g.categoriaId,
    data: formatarDataExibicao(g.data),
    dataIso: g.data
  }));
}

async function dbInserir(gasto) {
  const res = await API.post('/api/gastos', gasto);
  return {
    id: res.id,
    nome: res.nome,
    valor: res.valor,
    classe: res.categoriaNome || '',
    categoriaId: res.categoriaId,
    data: formatarDataExibicao(res.data),
    dataIso: res.data
  };
}

async function dbRemover(id) {
  await API.delete('/api/gastos/' + id);
}

async function dbAtualizar(id, dados) {
  const res = await API.put('/api/gastos/' + id, dados);
  return {
    id: res.id,
    nome: res.nome,
    valor: res.valor,
    classe: res.categoriaNome || '',
    categoriaId: res.categoriaId,
    data: formatarDataExibicao(res.data),
    dataIso: res.data
  };
}

async function dbBuscarGanhos() {
  const list = await API.get('/api/ganhos');
  return (list || []).map(g => ({
    id: g.id,
    nome: g.nome,
    valor: g.valor,
    classe: g.categoriaNome || '',
    categoriaId: g.categoriaId,
    fixo: g.fixo,
    data: formatarDataExibicao(g.data),
    dataIso: g.data
  }));
}

async function dbInserirGanho(ganho) {
  const res = await API.post('/api/ganhos', ganho);
  return {
    id: res.id,
    nome: res.nome,
    valor: res.valor,
    classe: res.categoriaNome || '',
    categoriaId: res.categoriaId,
    fixo: res.fixo,
    data: formatarDataExibicao(res.data),
    dataIso: res.data
  };
}

async function dbRemoverGanho(id) {
  await API.delete('/api/ganhos/' + id);
}

async function dbAtualizarGanho(id, dados) {
  const res = await API.put('/api/ganhos/' + id, dados);
  return {
    id: res.id,
    nome: res.nome,
    valor: res.valor,
    classe: res.categoriaNome || '',
    categoriaId: res.categoriaId,
    fixo: res.fixo,
    data: formatarDataExibicao(res.data),
    dataIso: res.data
  };
}

// ─── 5. LÓGICA DE GASTOS E GANHOS ─────────────────────
async function salvarGasto() {
  const nome   = document.getElementById('inp-nome').value;
  const valor  = document.getElementById('inp-valor').value;
  const classe = document.getElementById('inp-classe').value;
  const data   = document.getElementById('inp-data').value;

  const erros = [validarNome(nome), validarValor(valor), validarClasse(classe), validarData(data)];
  ['err-nome', 'err-valor', 'err-classe', 'err-data'].forEach((id, i) => setError(id, erros[i]));
  if (erros.some(e => e !== '')) return;

  const novoGasto = {
    nome: nome.trim(),
    valor: parseFloat(valor),
    categoriaId: parseInt(classe, 10),
    data
  };

  setBloqueadoFormulario(true);
  try {
    const registrado = await dbInserir(novoGasto);
    historico.push(registrado);
    atualizarStats();
    ['inp-nome', 'inp-valor', 'inp-data'].forEach(id => document.getElementById(id).value = '');
    document.getElementById('inp-classe').value = '';
    showToast(`✔ "${nome.trim()}" cadastrado!`, 'success');
  } catch (err) {
    showToast(err.message || 'Erro ao salvar.', 'danger');
  } finally {
    setBloqueadoFormulario(false);
  }
}

async function salvarGanho() {
  const nome   = document.getElementById('ganho-nome').value;
  const valor  = document.getElementById('ganho-valor').value;
  const classe = document.getElementById('ganho-classe').value;
  const data   = document.getElementById('ganho-data').value;
  const fixo   = document.getElementById('ganho-fixo').checked;

  const erros = [validarNome(nome), validarValor(valor), validarClasse(classe), validarData(data)];
  ['err-ganho-nome', 'err-ganho-valor', 'err-ganho-classe', 'err-ganho-data'].forEach((id, i) => setError(id, erros[i]));
  if (erros.some(e => e !== '')) return;

  const novoGanho = {
    nome: nome.trim(),
    valor: parseFloat(valor),
    categoriaId: parseInt(classe, 10),
    data,
    fixo
  };

  setBloqueadoFormulario(true);
  try {
    const registrado = await dbInserirGanho(novoGanho);
    historicoGanhos.push(registrado);
    atualizarStats();
    ['ganho-nome', 'ganho-valor', 'ganho-data'].forEach(id => document.getElementById(id).value = '');
    document.getElementById('ganho-classe').value = '';
    document.getElementById('ganho-fixo').checked = false;
    showToast(`✔ "${nome.trim()}" cadastrado!`, 'success');
  } catch (err) {
    showToast(err.message || 'Erro ao salvar ganho.', 'danger');
  } finally {
    setBloqueadoFormulario(false);
  }
}

async function confirmarRemocao() {
  const tipo = document.getElementById('modal-overlay').dataset.tipo;
  if (tipo === 'ganho') { await confirmarRemocaoGanho(); return; }

  if (idRemover === null) return;
  setBloqueadoFormulario(true);
  try {
    await dbRemover(idRemover);
    historico = historico.filter(g => g.id !== idRemover);
    fecharModal();
    renderListaRemover();
    atualizarStats();
    showToast('✔ Gasto removido!', 'danger');
  } catch (err) {
    showToast(err.message || 'Erro ao remover.', 'danger');
    fecharModal();
  } finally {
    setBloqueadoFormulario(false);
  }
}

async function confirmarRemocaoGanho() {
  if (idRemoverGanho === null) return;
  setBloqueadoFormulario(true);
  try {
    await dbRemoverGanho(idRemoverGanho);
    historicoGanhos = historicoGanhos.filter(g => g.id !== idRemoverGanho);
    idRemoverGanho = null;
    fecharModal();
    renderListaRemoverGanho();
    atualizarStats();
    showToast('✔ Ganho removido!', 'danger');
  } catch (err) {
    showToast(err.message || 'Erro ao remover.', 'danger');
    fecharModal();
  } finally {
    setBloqueadoFormulario(false);
  }
}

async function confirmarEdicao() {
  if (idEditar === null) return;

  const nome   = document.getElementById('edit-nome').value;
  const valor  = document.getElementById('edit-valor').value;
  const classe = document.getElementById('edit-classe').value;
  const data   = document.getElementById('edit-data').value;

  const erros = [validarNome(nome), validarValor(valor), validarClasse(classe), validarData(data)];
  ['err-edit-nome', 'err-edit-valor', 'err-edit-classe', 'err-edit-data'].forEach((id, i) => setError(id, erros[i]));
  if (erros.some(e => e !== '')) return;

  const dados = {
    nome: nome.trim(),
    valor: parseFloat(valor),
    categoriaId: parseInt(classe, 10),
    data
  };

  const btnSalvar = document.querySelector('#modal-editar-overlay .btn-primary');
  const btnCancelar = document.querySelector('#modal-editar-overlay .btn-cancel');
  if (btnSalvar) btnSalvar.disabled = true;
  if (btnCancelar) btnCancelar.disabled = true;

  try {
    const atualizado = await dbAtualizar(idEditar, dados);
    const idx = historico.findIndex(g => g.id === idEditar);
    if (idx !== -1) historico[idx] = atualizado;
    fecharModalEditar();
    renderListaEditar();
    atualizarStats();
    showToast('✔ Atualizado com sucesso!', 'success');
  } catch (err) {
    showToast(err.message || 'Erro ao atualizar.', 'danger');
  } finally {
    if (btnSalvar) btnSalvar.disabled = false;
    if (btnCancelar) btnCancelar.disabled = false;
  }
}

async function confirmarEdicaoGanho() {
  if (idEditarGanho === null) return;

  const nome   = document.getElementById('edit-ganho-nome').value;
  const valor  = document.getElementById('edit-ganho-valor').value;
  const classe = document.getElementById('edit-ganho-classe').value;
  const data   = document.getElementById('edit-ganho-data').value;
  const fixo   = document.getElementById('edit-ganho-fixo').checked;

  const erros = [validarNome(nome), validarValor(valor), validarClasse(classe), validarData(data)];
  ['err-edit-ganho-nome', 'err-edit-ganho-valor', 'err-edit-ganho-classe', 'err-edit-ganho-data'].forEach((id, i) => setError(id, erros[i]));
  if (erros.some(e => e !== '')) return;

  const dados = {
    nome: nome.trim(),
    valor: parseFloat(valor),
    categoriaId: parseInt(classe, 10),
    data,
    fixo
  };

  const btnSalvar = document.querySelector('#modal-editar-ganho-overlay .btn-primary');
  const btnCancelar = document.querySelector('#modal-editar-ganho-overlay .btn-cancel');
  if (btnSalvar) btnSalvar.disabled = true;
  if (btnCancelar) btnCancelar.disabled = true;

  try {
    const atualizado = await dbAtualizarGanho(idEditarGanho, dados);
    const idx = historicoGanhos.findIndex(g => g.id === idEditarGanho);
    if (idx !== -1) historicoGanhos[idx] = atualizado;
    fecharModalEditarGanho();
    renderListaEditarGanho();
    atualizarStats();
    showToast('✔ Ganho atualizado com sucesso!', 'success');
  } catch (err) {
    showToast(err.message || 'Erro ao atualizar ganho.', 'danger');
  } finally {
    if (btnSalvar) btnSalvar.disabled = false;
    if (btnCancelar) btnCancelar.disabled = false;
  }
}

// ─── 6. STATS E RENDERIZAÇÃO ──────────────────────────
function atualizarStats() {
  const totalGastos  = historico.reduce((s, g) => s + Number(g.valor), 0);
  const totalGanhos  = historicoGanhos.reduce((s, g) => s + Number(g.valor), 0);
  const maxVal       = historico.length ? Math.max(...historico.map(g => Number(g.valor))) : 0;
  const balanco      = totalGanhos - totalGastos;

  document.getElementById('stat-total').textContent  = `R$ ${formatVal(totalGastos)}`;
  document.getElementById('stat-count').textContent  = historico.length;
  document.getElementById('stat-max').textContent    = `R$ ${formatVal(maxVal)}`;

  const cardTotal  = document.getElementById('stat-total').parentElement;
  const cardLimite = document.getElementById('card-limite');
  if (cardTotal)  cardTotal.classList.remove('alerta-atencao', 'alerta-critico');
  if (cardLimite) cardLimite.classList.remove('alerta-atencao', 'alerta-critico');

  if (limiteMensal > 0) {
    const p = (totalGastos / limiteMensal) * 100;
    if (p >= 100) {
      if (cardTotal)  cardTotal.classList.add('alerta-critico');
      if (cardLimite) cardLimite.classList.add('alerta-critico');
      showToast('⚠️ Limite de gastos excedido!', 'danger');
    } else if (p >= 80) {
      if (cardTotal)  cardTotal.classList.add('alerta-atencao');
      if (cardLimite) cardLimite.classList.add('alerta-atencao');
    }
  }

  const elBalanco = document.getElementById('stat-balanco');
  if (elBalanco) {
    elBalanco.textContent = `R$ ${formatVal(balanco)}`;
    elBalanco.style.color = balanco >= 0 ? 'var(--success)' : 'var(--danger)';
  }

  window.dispatchEvent(new Event('dadosAtualizados'));
}

function renderRelatorio() {
  const cont = document.getElementById('tbody-relatorio');
  if (!cont) return;
  if (!historico.length) {
    document.getElementById('empty-relatorio').style.display = 'block';
    cont.innerHTML = '';
    document.getElementById('total-rel').textContent = 'R$ 0,00';
    return;
  }
  document.getElementById('empty-relatorio').style.display = 'none';
  cont.innerHTML = historico.map(g => `
    <tr>
      <td>${escHtml(g.nome)}</td>
      <td class="val-cell">R$ ${formatVal(Number(g.valor))}</td>
      <td><span class="tag-categoria">${escHtml(g.classe)}</span></td>
      <td>${escHtml(g.data)}</td>
    </tr>`).join('');
  const total = historico.reduce((s, g) => s + Number(g.valor), 0);
  document.getElementById('total-rel').textContent = `R$ ${formatVal(total)}`;
}

function renderListaRemover() {
  const cont = document.getElementById('lista-remover');
  if (!cont) return;
  if (!historico.length) {
    cont.innerHTML = `<div class="empty-state"><div class="empty-icon">⬡</div><div class="empty-text">Nenhum gasto</div></div>`; return;
  }
  cont.innerHTML = historico.map(g => `
    <div class="report-card">
      <div class="report-row">
        <div><div class="report-name">${escHtml(g.nome)}</div><div class="report-meta">${escHtml(g.classe)} · ${escHtml(g.data)}</div></div>
        <div style="display:flex; align-items:center; gap:14px">
          <span class="val-cell">R$ ${formatVal(Number(g.valor))}</span>
          <button class="btn-remove" onclick="abrirModal(${g.id}, '${escHtml(g.nome)}')">✕ Remover</button>
        </div>
      </div>
    </div>`).join('');
}

function renderListaRemoverGanho() {
  const cont = document.getElementById('lista-remover-ganho');
  if (!cont) return;
  if (!historicoGanhos.length) {
    cont.innerHTML = `<div class="empty-state"><div class="empty-icon">⬡</div><div class="empty-text">Nenhum ganho</div></div>`; return;
  }
  cont.innerHTML = historicoGanhos.map(g => `
    <div class="report-card">
      <div class="report-row">
        <div><div class="report-name">${escHtml(g.nome)}</div><div class="report-meta">${escHtml(g.classe)} · ${escHtml(g.data)}${g.fixo ? ' · Fixo' : ''}</div></div>
        <div style="display:flex; align-items:center; gap:14px">
          <span class="val-cell val-ganho">R$ ${formatVal(Number(g.valor))}</span>
          <button class="btn-remove" onclick="abrirModalGanho(${g.id}, '${escHtml(g.nome)}')">✕ Remover</button>
        </div>
      </div>
    </div>`).join('');
}

function renderListaEditar() {
  const cont = document.getElementById('lista-editar');
  if (!cont) return;
  if (!historico.length) {
    cont.innerHTML = `<div class="empty-state"><div class="empty-icon">⬡</div><div class="empty-text">Nenhum gasto</div></div>`; return;
  }
  cont.innerHTML = historico.map(g => `
    <div class="report-card">
      <div class="report-row">
        <div><div class="report-name">${escHtml(g.nome)}</div><div class="report-meta">${escHtml(g.classe)} · ${escHtml(g.data)}</div></div>
        <div style="display:flex; align-items:center; gap:14px">
          <span class="val-cell">R$ ${formatVal(Number(g.valor))}</span>
          <button class="btn-edit" onclick="abrirModalEditar(${g.id})">✎ Editar</button>
        </div>
      </div>
    </div>`).join('');
}

function renderListaEditarGanho() {
  const cont = document.getElementById('lista-editar-ganho');
  if (!cont) return;
  if (!historicoGanhos.length) {
    cont.innerHTML = `<div class="empty-state"><div class="empty-icon">⬡</div><div class="empty-text">Nenhum ganho para editar</div></div>`; return;
  }
  cont.innerHTML = historicoGanhos.map(g => `
    <div class="report-card">
      <div class="report-row">
        <div><div class="report-name">${escHtml(g.nome)}</div><div class="report-meta">${escHtml(g.classe)} · ${escHtml(g.data)}${g.fixo ? ' · Fixo' : ''}</div></div>
        <div style="display:flex; align-items:center; gap:14px">
          <span class="val-cell val-ganho">R$ ${formatVal(Number(g.valor))}</span>
          <button class="btn-edit btn-ganho" onclick="abrirModalEditarGanho(${g.id})">✎ Editar</button>
        </div>
      </div>
    </div>`).join('');
}

// ─── 7. MODAIS E NAVEGAÇÃO ────────────────────────────
function showView(view, btn) {
  document.querySelectorAll('.view').forEach(v => v.classList.remove('active'));
  document.querySelectorAll('.nav-btn').forEach(b => b.classList.remove('active'));
  document.getElementById('view-' + view).classList.add('active');
  btn.classList.add('active');

  if (view === 'remover')       renderListaRemover();
  if (view === 'remover-ganho') renderListaRemoverGanho();
  if (view === 'editar')        renderListaEditar();
  if (view === 'editar-ganho')  renderListaEditarGanho();
  if (view === 'relatorio')     renderRelatorio();
}

function abrirModal(id, nome) {
  idRemover = id;
  document.getElementById('modal-nome').textContent = nome;
  document.getElementById('modal-overlay').classList.add('open');
}

function abrirModalGanho(id, nome) {
  idRemoverGanho = id;
  document.getElementById('modal-nome').textContent = nome;
  const overlay = document.getElementById('modal-overlay');
  overlay.classList.add('open');
  overlay.dataset.tipo = 'ganho';
}

function fecharModal() {
  idRemover = null;
  idRemoverGanho = null;
  const overlay = document.getElementById('modal-overlay');
  overlay.classList.remove('open');
  delete overlay.dataset.tipo;
}

function abrirModalEditar(id) {
  const g = historico.find(x => x && x.id === id);
  if (!g) return;
  idEditar = id;
  document.getElementById('edit-nome').value   = g.nome;
  document.getElementById('edit-valor').value  = g.valor;
  document.getElementById('edit-classe').value = g.categoriaId;
  document.getElementById('edit-data').value   = formatarDataIso(g.dataIso || g.data);
  ['err-edit-nome', 'err-edit-valor', 'err-edit-classe', 'err-edit-data'].forEach(x => setError(x, ''));
  document.getElementById('modal-editar-overlay').classList.add('open');
}

function fecharModalEditar() {
  idEditar = null;
  document.getElementById('modal-editar-overlay').classList.remove('open');
}

function abrirModalEditarGanho(id) {
  const g = historicoGanhos.find(x => x && x.id === id);
  if (!g) return;
  idEditarGanho = id;
  document.getElementById('edit-ganho-nome').value   = g.nome;
  document.getElementById('edit-ganho-valor').value  = g.valor;
  document.getElementById('edit-ganho-classe').value = g.categoriaId;
  document.getElementById('edit-ganho-data').value   = formatarDataIso(g.dataIso || g.data);
  document.getElementById('edit-ganho-fixo').checked = !!g.fixo;
  ['err-edit-ganho-nome', 'err-edit-ganho-valor', 'err-edit-ganho-classe', 'err-edit-ganho-data'].forEach(x => setError(x, ''));
  document.getElementById('modal-editar-ganho-overlay').classList.add('open');
}

function fecharModalEditarGanho() {
  idEditarGanho = null;
  document.getElementById('modal-editar-ganho-overlay').classList.remove('open');
}

// ─── 8. LIMITE MENSAL ─────────────────────────────────
async function carregarLimiteSalvo() {
  try {
    const data = await API.get('/api/limite');
    limiteMensal = data ? (parseFloat(data.valor) || 0) : 0;
  } catch {
    limiteMensal = 0;
  }

  const inp = document.getElementById('inp-limite');
  if (inp) inp.value = limiteMensal > 0 ? limiteMensal : '';

  const el = document.getElementById('stat-limite');
  if (el) el.textContent = `R$ ${formatVal(limiteMensal)}`;
}

async function salvarLimiteMensal() {
  const inp = document.getElementById('inp-limite');
  if (!inp) { showToast('Campo de limite não encontrado.', 'danger'); return; }

  const val = parseFloat(inp.value);
  if (inp.value === '' || isNaN(val) || val < 0) {
    showToast('Informe um valor válido para o limite.', 'danger'); return;
  }

  try {
    const res = await API.put('/api/limite', { valor: val });
    limiteMensal = res ? parseFloat(res.valor) : val;

    const el = document.getElementById('stat-limite');
    if (el) el.textContent = `R$ ${formatVal(limiteMensal)}`;
    atualizarStats();
    showToast('✔ Limite salvo!', 'success');
  } catch (err) {
    showToast(err.message || 'Erro ao salvar limite.', 'danger');
  }
}

// ─── 9. GOOGLE AUTH (GSI) ─────────────────────────────
function inicializarGoogleAuth() {
  if (!window.google || typeof CONFIG === 'undefined' || !CONFIG.GOOGLE_CLIENT_ID) return;

  try {
    google.accounts.id.initialize({
      client_id: CONFIG.GOOGLE_CLIENT_ID,
      callback: handleGoogleCredentialResponse,
    });

    const btnLogin = document.getElementById('google-btn-login');
    if (btnLogin) {
      google.accounts.id.renderButton(btnLogin, {
        theme: 'outline',
        size: 'large',
        text: 'signin_with',
        shape: 'rectangular',
        width: 280,
      });
    }

    const btnReg = document.getElementById('google-btn-reg');
    if (btnReg) {
      google.accounts.id.renderButton(btnReg, {
        theme: 'outline',
        size: 'large',
        text: 'signup_with',
        shape: 'rectangular',
        width: 280,
      });
    }
  } catch (e) {
    console.warn('Google Identity Services não inicializado:', e);
  }
}

async function handleGoogleCredentialResponse(response) {
  if (!response || !response.credential) return;

  setBloqueadoAuth(true);
  try {
    const dados = await API.post('/api/auth/google', { idToken: response.credential });
    API.setToken(dados.token);

    let email = 'Google User';
    try {
      const payload = JSON.parse(atob(dados.token.split('.')[1]));
      if (payload.email) email = payload.email;
    } catch {}

    API.setEmail(email);
    await entrarNoApp(email);
  } catch (err) {
    setError('err-login-geral', err.message || 'Erro ao autenticar com Google.');
    setError('err-reg-geral', err.message || 'Erro ao autenticar com Google.');
  } finally {
    setBloqueadoAuth(false);
  }
}

// ─── 10. UTILITÁRIOS ──────────────────────────────────
function validarNome(v)   { return (!v || v.trim().length === 0) ? 'Informe um nome.' : (/^\d+$/.test(v.trim()) ? 'Apenas números não é válido.' : ''); }
function validarValor(v)  { const n = parseFloat(v); return (isNaN(n) || n <= 0) ? 'Valor inválido.' : ''; }
function validarClasse(v) { return (!v || String(v).trim().length === 0) ? 'Informe a categoria.' : ''; }
function validarData(v)   { return !v ? 'Selecione a data.' : ''; }
function setError(id, msg){ const el = document.getElementById(id); if (el) el.textContent = msg; }
function formatVal(n)     { return Number(n).toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }); }
function escHtml(s)       { return String(s).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;'); }

function setBloqueadoFormulario(e) {
  const b = document.querySelectorAll('#view-cadastro .btn-primary, #view-cadastro-ganho .btn-primary, #modal-overlay button');
  b.forEach(x => {
    x.disabled = e;
    x.style.opacity = e ? '0.5' : '1';
    x.style.cursor = e ? 'not-allowed' : 'pointer';
  });
}

function setBloqueadoAuth(e) {
  const b = document.querySelectorAll('.btn-auth');
  b.forEach(x => {
    x.disabled = e;
    x.style.opacity = e ? '0.5' : '1';
    x.style.cursor = e ? 'not-allowed' : 'pointer';
  });
}

function showToast(msg, type = '') {
  const t = document.getElementById('toast');
  if (!t) return;
  t.textContent = msg;
  t.className = 'toast' + (type ? ' ' + type : '');
  t.classList.add('show');
  setTimeout(() => t.classList.remove('show'), 3000);
}

// ─── 11. INICIALIZAÇÃO ────────────────────────────────
window.addEventListener('DOMContentLoaded', async () => {
  const token = API.getToken();
  const email = API.getEmail();
  if (token && email) {
    try {
      await entrarNoApp(email);
    } catch {
      fazerLogout();
    }
  }
});

window.addEventListener('load', () => {
  setTimeout(inicializarGoogleAuth, 400);
});
