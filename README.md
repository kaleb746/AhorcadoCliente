# 🎮 Ahorcado - Cliente

## 📌 Descripción

**Ahorcado Cliente** es la aplicación encargada de interactuar con el usuario final del juego Ahorcado. Está desarrollada en **C# utilizando .NET Framework** y consume los servicios expuestos por el proyecto **AhorcadoServidor** mediante **WCF (Windows Communication Foundation)**.

Este proyecto implementa la interfaz de usuario y la comunicación con el servidor bajo un modelo cliente-servidor, permitiendo a los jugadores registrarse, iniciar sesión y participar en partidas del juego.

---

## 📚 Contexto Académico

Este proyecto fue desarrollado como parte de una práctica académica enfocada en el desarrollo de sistemas distribuidos utilizando arquitectura cliente-servidor y consumo de servicios WCF.

---

## ✨ Características

### 🖥️ Aplicación Cliente

- Interfaz para interacción con el usuario.
- Conexión remota al servidor mediante WCF.
- Consumo de métodos expuestos por el servicio.
- Manejo de sesiones de usuario.

---

## 🔗 Comunicación con el Servidor

El cliente utiliza una **Referencia de Servicio (Service Reference)** para generar un proxy que permite invocar métodos remotos como si fueran locales.

Algunas operaciones disponibles:

- 🔐 Registro de jugador  
- 🔑 Inicio de sesión  
- 🎮 Creación de partidas  
- 👥 Unión a partidas existentes  
- 🔤 Intento de letras  
- 📊 Consulta del estado de la partida  
- 📜 Consulta de historial  

---

## 🛠️ Tecnologías Utilizadas

- C#
- .NET Framework
- WCF (Windows Communication Foundation)
- Arquitectura Cliente-Servidor

---

## 📂 Estructura del Proyecto
AhorcadoCliente
│
├── AhorcadoCliente.sln     
├── AhorcadoCliente/
│   ├── Program.cs           
│   ├── App.config              
│   ├── ServicioAhorcadoRef/     
│   │   ├── Reference.svcmap
│   │   ├── Reference.cs
│   │   └── config
│   ├── Controllers/          
│   ├── Views/                   
│   └── Utils/                   
│
├── README.md  
└── .gitignore             

---

## ▶️ Ejecución del Proyecto

1. Asegurarse de que el proyecto **AhorcadoServidor** esté en ejecución.
2. Clonar el repositorio:

```bash
git clone https://github.com/kaleb746/AhorcadoCliente.git
