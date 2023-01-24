using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiAutores.Controllers.V1;
using WebAPIAutores.Test.Mocks;

namespace WebAPIAutores.Test.PruebasUnitarias
{
    [TestClass]
    public class RootControllerTest
    {
        [TestMethod]
        public async Task SiUsuarioEsAdmin_Obtenemos4links()
        { 
        
        //Preparacion
        var authorizationService = new AuthorizationServicesMock();
            authorizationService.Resultado = AuthorizationResult.Success();
        var rootController = new RootController(authorizationService);
            rootController.Url = new URLHelperMock();

        //Ejecucion
        var resultado = await rootController.Get();
            //Verificacion
            Assert.AreEqual(4, resultado.Value.Count());
        }
        [TestMethod]
        public async Task SiUsuarioNoEsAdmin_Obtenemos2links()
        {

            //Preparacion
            var authorizationService = new AuthorizationServicesMock();
            authorizationService.Resultado = AuthorizationResult.Failed();
            var rootController = new RootController(authorizationService);
            rootController.Url = new URLHelperMock();

            //Ejecucion
            var resultado = await rootController.Get();
            //Verificacion
            Assert.AreEqual(2, resultado.Value.Count());
        }
    }
}
