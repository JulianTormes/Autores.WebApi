﻿using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace WebApiAutores.Utilidades
{
    public class CabeceraEstaPresenteAttribute : Attribute, IActionConstraint
    {
        private readonly string _cabecera;
        private readonly string _valor;

        public CabeceraEstaPresenteAttribute(String cabecera, String Valor)
        {
            _cabecera = cabecera;
            _valor = Valor;
        }
        public int Order => 0;

        public bool Accept(ActionConstraintContext context)
        {
            var cabeceras = context.RouteContext.HttpContext.Request.Headers;

            if (!cabeceras.ContainsKey(_cabecera))
            {
                return false;
            }

            return string.Equals(cabeceras[_cabecera],_valor, StringComparison.OrdinalIgnoreCase);
        }
    }
}
