// =====================================================
// api.js — Cliente HTTP para comunicação com a API REST
// =====================================================

const API = {
  getToken() {
    return sessionStorage.getItem('token_auth');
  },

  setToken(token) {
    if (token) {
      sessionStorage.setItem('token_auth', token);
    } else {
      sessionStorage.removeItem('token_auth');
    }
  },

  getEmail() {
    return sessionStorage.getItem('user_email') || '';
  },

  setEmail(email) {
    if (email) {
      sessionStorage.setItem('user_email', email);
    } else {
      sessionStorage.removeItem('user_email');
    }
  },

  async request(endpoint, options = {}) {
    const baseUrl = (typeof CONFIG !== 'undefined' && CONFIG.API_URL) ? CONFIG.API_URL : 'http://localhost:5065';
    const url = `${baseUrl}${endpoint}`;

    const headers = {
      'Content-Type': 'application/json',
      ...(options.headers || {})
    };

    const token = this.getToken();
    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }

    const fetchConfig = {
      ...options,
      headers
    };

    let res;
    try {
      res = await fetch(url, fetchConfig);
    } catch {
      throw new Error('Erro de conexão com o servidor.');
    }

    if (res.status === 401) {
      // Se é uma chamada de login/register, NÃO tratar como sessão expirada
      const isAuthEndpoint = endpoint.startsWith('/api/auth/');
      if (!isAuthEndpoint) {
        this.setToken(null);
        this.setEmail(null);
        if (typeof fazerLogout === 'function') {
          fazerLogout(true);
        }
        throw new Error('Sessão expirada. Faça login novamente.');
      }
      // Para endpoints de auth, deixar cair no tratamento genérico abaixo
    }

    if (res.status === 204) {
      return null;
    }

    const contentType = res.headers.get('content-type') || '';
    let data = null;
    if (contentType.includes('application/json') || contentType.includes('application/problem+json')) {
      data = await res.json();
    } else {
      data = await res.text();
    }

    if (!res.ok) {
      const msg = (data && (data.detail || data.message || data.title)) || `Erro HTTP ${res.status}`;
      const error = new Error(msg);
      error.status = res.status;
      error.data = data;
      throw error;
    }

    return data;
  },

  get(endpoint) {
    return this.request(endpoint, { method: 'GET' });
  },

  post(endpoint, body) {
    return this.request(endpoint, { method: 'POST', body: JSON.stringify(body) });
  },

  put(endpoint, body) {
    return this.request(endpoint, { method: 'PUT', body: JSON.stringify(body) });
  },

  delete(endpoint) {
    return this.request(endpoint, { method: 'DELETE' });
  }
};

