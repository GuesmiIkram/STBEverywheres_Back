﻿using STBEverywhere_Back_SharedModels;
using STBEverywhere_Back_SharedModels.Models.DTO;



namespace STBEverywhere_back_APIClient.Services
{
    public interface IClientService
    {

        Task<Client> GetClientByIdAsync(int clientId);
        Task<bool> UpdateClientInfoAsync(int clientId, Client updatedClient);
        Task<string> RegisterAsync(RegisterDto registerDto);
        Task<bool> UploadProfileImageAsync(int clientId, string fileName);
        Task<bool> RemoveProfileImageAsync(int clientId);

    }
}
