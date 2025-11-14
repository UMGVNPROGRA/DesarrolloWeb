using System.Net.Http.Json;
using HospitalQueueSystem.Models;

namespace HospitalQueueSystem.Blazor.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        // ✅ Constructor que recibe IHttpClientFactory
        public AuthService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("HospitalAPI");
            Console.WriteLine($"✅ AuthService inicializado con HttpClient nombrado");
            Console.WriteLine($"✅ BaseAddress: {_httpClient.BaseAddress}");
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            try
            {
                Console.WriteLine("=== 🔍 INICIANDO PETICIÓN LOGIN ===");
                Console.WriteLine($"📧 Email: {request.Email}");
                Console.WriteLine($"🔑 Password: {request.Password}");
                Console.WriteLine($"🌐 URL destino: {_httpClient.BaseAddress}api/auth/login");
                Console.WriteLine($"🕐 Hora: {DateTime.Now:HH:mm:ss}");

                // Mostrar el JSON que se enviará
                var jsonContent = System.Text.Json.JsonSerializer.Serialize(request);
                Console.WriteLine($"📦 Body Request JSON: {jsonContent}");

                // ✅ Hacer la petición con ruta relativa (BaseAddress ya configurado)
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);

                Console.WriteLine($"📡 Response Status: {(int)response.StatusCode} {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    Console.WriteLine($"✅ LOGIN EXITOSO");
                    Console.WriteLine($"✅ Usuario: {result?.Nombre}");
                    Console.WriteLine($"✅ Rol: {result?.Rol}");
                    Console.WriteLine($"✅ Token: {result?.Token?.Substring(0, 20)}...");
                    return result;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ ERROR DEL SERVIDOR: {errorContent}");
                    Console.WriteLine($"❌ Status Code: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 EXCEPCIÓN EN AuthService:");
                Console.WriteLine($"💥 Mensaje: {ex.Message}");
                Console.WriteLine($"💥 Tipo: {ex.GetType().Name}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine($"💥 Inner Exception: {ex.InnerException.Message}");
                }
                return null;
            }
            finally
            {
                Console.WriteLine("=== 🏁 FIN PETICIÓN LOGIN ===\n");
            }
        }

        public void SetAuthToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                Console.WriteLine($"🔐 Token configurado en HttpClient");
            }
        }
    }
}