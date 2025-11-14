using System.Net.Http.Json;
using HospitalQueueSystem.Models;

namespace HospitalQueueSystem.Blazor.Services
{
    public class TurnosService
    {
        private readonly HttpClient _httpClient;

        public TurnosService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("HospitalAPI");
            Console.WriteLine($"✅ TurnosService inicializado con HttpClient nombrado");
            Console.WriteLine($"✅ BaseAddress: {_httpClient.BaseAddress}");
        }

        public async Task<List<Turno>?> GetTurnosAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Turno>>("api/turnos");
        }

        public async Task<List<Turno>?> GetTurnosPorClinicaAsync(int clinicaId)
        {
            return await _httpClient.GetFromJsonAsync<List<Turno>>($"api/turnos/clinica/{clinicaId}");
        }

        public async Task<List<Clinica>?> GetClinicasAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Clinica>>("api/clinicas");
        }

        public async Task<Turno?> CrearTurnoAsync(Turno turno)
        {
            var response = await _httpClient.PostAsJsonAsync("api/turnos", turno);
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<Turno>() : null;
        }

        public async Task<bool> LlamarTurnoAsync(int turnoId)
        {
            var response = await _httpClient.PostAsync($"api/turnos/{turnoId}/llamar", null);
            return response.IsSuccessStatusCode;
        }
    }
}