BreakingDesingStock

<p align="center">
  <img src="assets/logo.png" alt="Logo de Breaking Desing" width="200">
</p>

<h3 align="center">Sistema de gestión de stock y ventas para emprendimientos de impresión 3D</h3>

<p align="center">
  <a href="http://breakingdesing.runasp.net/">
    <img src="https://img.shields.io/badge/VISITAR_APLICACIÓN-BreakingDesingStock-blue?style=for-the-badge" alt="Visitar aplicación">
  </a>
</p>

Descripción

BreakingDesingStock es una aplicación web desarrollada para Breaking Desing, un emprendimiento dedicado a la impresión 3D.

El proyecto nació con el objetivo de centralizar y simplificar la administración del negocio, permitiendo gestionar productos, controlar el inventario de filamentos, registrar ventas y consultar información importante desde cualquier dispositivo con acceso a internet.

Funcionalidades

Gestión de productos: alta, edición y administración de productos, categorías, precios y stock.

Control de inventario: seguimiento del stock disponible y configuración de niveles mínimos.

Gestión de filamentos: administración de materiales, colores y precios por kilogramo.

Consumo de materiales: asociación de filamentos a productos y registro de gramos utilizados y desperdiciados.

Registro de ventas: almacenamiento de operaciones de venta, cantidades, fechas y precios.

Control de stock: seguimiento de las existencias de productos para facilitar la gestión del emprendimiento.

Acceso multiplataforma: interfaz web accesible desde computadoras, tablets y celulares.

Tecnologías utilizadas

Tecnología

Uso

C#

Lenguaje de programación principal

ASP.NET Core 8

Desarrollo de la aplicación web

Razor Components / Blazor

Interfaz de usuario interactiva

ADO.NET

Acceso y comunicación con SQL Server

SQL Server

Gestión y almacenamiento de datos

Dapper

Consultas y operaciones con la base de datos

HTML / CSS

Estructura y estilos de la interfaz

Base de datos

La aplicación utiliza SQL Server para almacenar y organizar la información del emprendimiento.

Las principales entidades del sistema son:

Filamentos: materiales disponibles para impresión.

Productos: catálogo de productos y sus existencias.

ProductoFilamentos: relación entre productos y los materiales que utilizan.

Ventas: historial de ventas realizadas.

Demo online

Podés acceder a una versión publicada de la aplicación desde el siguiente enlace:

Visitar BreakingDesingStock

La aplicación está pensada como una herramienta de gestión interna para el emprendimiento Breaking Desing. La demo se ofrece con fines demostrativos.

Ejecución local

Para ejecutar el proyecto en un entorno local necesitás:

.NET 8 SDK

SQL Server

Visual Studio 2022 o un entorno compatible con .NET 8

Pasos

Clonar el repositorio:

git clone https://github.com/TU-USUARIO/TU-REPOSITORIO.git

Abrir la solución en Visual Studio.

Crear la base de datos SQL Server utilizando el script de estructura incluido en el proyecto.

Configurar la cadena de conexión en appsettings.json:

{
  "ConnectionStrings": {
    "Default": "Server=TU_SERVIDOR;Database=StockImpresiones3D;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}

Restaurar los paquetes y ejecutar la aplicación:

dotnet restore
dotnet run

Importante: reemplazá los datos de conexión por los de tu propio entorno. Nunca publiques contraseñas ni credenciales reales en el repositorio.

Objetivo del proyecto

Este proyecto fue desarrollado para resolver necesidades reales de un emprendimiento de impresión 3D, aplicando conocimientos de desarrollo web, programación orientada a objetos, acceso a datos y diseño de bases de datos relacionales.

Además de ser una herramienta de uso personal para Breaking Desing, representa un proyecto práctico de desarrollo con tecnologías del ecosistema .NET.

<p align="center">
  Desarrollado con C# y ASP.NET Core 8
  <br>
  <strong>Breaking Desing</strong>
</p>
