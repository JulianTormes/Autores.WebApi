using AutoMapper.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Runtime.ConstrainedExecution;
using WebApiAutores.Validaciones;

namespace WebAPIAutores.Test.PruebasUnitarias
{
    [TestClass]
    public class PrimeraLetraMayusculaAttributeTests
    {
        [TestMethod]
        public void PrimeraLetraMinuscula_DevuelveError()
        {
            //Preparacion
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            var valor = "felipe";
            var valContext = new System.ComponentModel.DataAnnotations.ValidationContext(new {Nombre= valor });

            //Ejecuccion
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);
            //Verificacion
            Assert.AreEqual("La primera letra debe ser mayuscula", resultado.ErrorMessage);
        }
        [TestMethod]
        public void ValorNulo_NoDevuelveError ()
        {
            //Preparacion
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            string valor = null;
            var valContext = new System.ComponentModel.DataAnnotations.ValidationContext(new { Nombre = valor });

            //Ejecuccion
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);
            //Verificacion
            Assert.IsNull(resultado);
        }
        [TestMethod]
        public void ValorConPrimerLetraMayuscula_NoDevuelveError()
        {
            //Preparacion
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            string valor = "Julian";
            var valContext = new System.ComponentModel.DataAnnotations.ValidationContext(new { Nombre = valor });

            //Ejecuccion
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);
            //Verificacion
            Assert.IsNull(resultado);
        }
    }
}