using Microsoft.AspNetCore.Mvc;

namespace STBEverywhere_back_APICompte.Services
{
    public interface IVirementService
    {

        Task<IActionResult> VirementDeMasseTraitement([FromBody] string fichier);
            
    }

}
