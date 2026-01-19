using TR.Connectors.Api.Entities;

namespace TR.Connectors.Api.Interfaces;

/// <summary>
/// Интерфейс коннектора для интеграции с внешней системой управления пользователями и доступами.
/// Перед вызовом остальных методов необходимо вызвать <see cref="StartUp"/>.
/// </summary>
public interface IConnector
{
    /// <summary>
    /// Логгер, используемый реализацией коннектора для вывода диагностических сообщений.
    /// Должен быть установлен вызывающим кодом до вызова <see cref="StartUp"/>.
    /// </summary>
    ILogger Logger { get; set; }

    /// <summary>
    /// Инициализирует коннектор и выполняет аутентификацию
    /// с использованием переданной строки подключения.
    /// </summary>
    /// <param name="connectionString">
    /// Строка подключения в формате, специфичном для реализации
    /// (например: "url=...;login=...;password=...").
    /// </param>
    void StartUp(string connectionString);

    /// <summary>
    /// Проверяет, существует ли пользователь с указанным логином во внешней системе.
    /// </summary>
    /// <param name="userLogin">Логин пользователя.</param>
    /// <returns><c>true</c>, если пользователь существует; иначе <c>false</c>.</returns>
    bool IsUserExists(string userLogin);

    /// <summary>
    /// Создаёт нового пользователя во внешней системе.
    /// </summary>
    /// <param name="user">Данные пользователя для создания.</param>
    void CreateUser(UserToCreate user);

    /// <summary>
    /// Возвращает список всех доступных пользовательских свойств,
    /// поддерживаемых внешней системой.
    /// </summary>
    IEnumerable<Property> GetAllProperties();

    /// <summary>
    /// Возвращает свойства пользователя с указанным логином.
    /// </summary>
    /// <param name="userLogin">Логин пользователя.</param>
    /// <returns>Коллекция свойств пользователя.</returns>
    IEnumerable<UserProperty> GetUserProperties(string userLogin);

    /// <summary>
    /// Обновляет свойства пользователя во внешней системе.
    /// </summary>
    /// <param name="properties">Список свойств для обновления.</param>
    /// <param name="userLogin">Логин пользователя.</param>
    void UpdateUserProperties(IEnumerable<UserProperty> properties, string userLogin);

    /// <summary>
    /// Возвращает список всех доступных прав и ролей,
    /// поддерживаемых внешней системой.
    /// </summary>
    IEnumerable<Permission> GetAllPermissions();

    /// <summary>
    /// Возвращает идентификаторы прав, назначенных пользователю.
    /// </summary>
    /// <param name="userLogin">Логин пользователя.</param>
    /// <returns>Коллекция идентификаторов прав.</returns>
    IEnumerable<string> GetUserPermissions(string userLogin);

    /// <summary>
    /// Назначает пользователю указанные права.
    /// </summary>
    /// <param name="userLogin">Логин пользователя.</param>
    /// <param name="rightIds">
    /// Коллекция идентификаторов прав в формате,
    /// специфичном для реализации (например: "ItRole,5", "RequestRight,3").
    /// </param>
    void AddUserPermissions(string userLogin, IEnumerable<string> rightIds);

    /// <summary>
    /// Удаляет указанные права у пользователя.
    /// </summary>
    /// <param name="userLogin">Логин пользователя.</param>
    /// <param name="rightIds">
    /// Коллекция идентификаторов прав в формате,
    /// специфичном для реализации.
    /// </param>
    void RemoveUserPermissions(string userLogin, IEnumerable<string> rightIds);
}
