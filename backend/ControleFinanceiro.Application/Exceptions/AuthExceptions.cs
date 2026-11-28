namespace ControleFinanceiro.Application.Exceptions;

public class NaoEncontradoException(string message = "Recurso não encontrado.") : Exception(message);

public class ValidacaoException(string message) : Exception(message);

public class ConflitoException(string message = "Conflito de recursos.") : Exception(message);

public class CredenciaisInvalidasException(string message = "credenciais inválidas") : Exception(message);

public class ProibidoException(string message = "Acesso proibido.") : Exception(message);

// Aliases para manter compatibilidade
public class NotFoundException(string message = "Recurso não encontrado.") : NaoEncontradoException(message);
public class ValidationException(string message) : ValidacaoException(message);
public class EmailAlreadyRegisteredException(string message = "E-mail já cadastrado.") : ConflitoException(message);
public class InvalidCredentialsException(string message = "credenciais inválidas") : CredenciaisInvalidasException(message);
