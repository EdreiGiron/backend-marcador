# Documentación Proyecto I : Marcador de baloncesto en tiempo real

## 1. Introducción
Este proyecto consiste en el desarrollo de una aplicación web que simula un marcador de baloncesto en tiempo real, permitiendo gestionar:
- Puntos por equipo.
- Control de cuartos.
- Temporizador con inicio/pausa/reinicio.
- Registro de faltas.
- Interfaz intuitiva y responsiva.

El objetivo principal es ofrecer una herramienta sencilla pero completa que cumpla con las reglas básicas del baloncesto y pueda ejecutarse en entornos web modernos, con separación entre la vista del tablero y el controlador, lo cual era opcional pero para un mejor orden, esto se aplicó en el proyecto.

---

## 2. Requisitos Funcionales
- **Marcador de puntos**
  - Mostrar puntos de cada equipo, es decir local y visitante.
  - Sumar puntos o restar puntos en caso de error.
- **Gestión de tiempo**
  - Temporizador configurable, en este caso 10 min por cuarto.
  - Botones de inicio, pausa y reinicio.
  - Notificación visual al terminar.
- **Cuartos**
  - Indicar el cuarto actual, de 1 a 4.
  - Avance manual o automático de cuarto.
- **Faltas**
  - Contador de faltas por equipo.
- **Control general**
  - Reinicio de partido, como puntos, faltas, tiempo.
  - Interfaz responsiva y clara.

---

## 3. Requisitos No Funcionales
- **Usabilidad**: interfaz clara, botones visibles y etiquetados.
- **Compatibilidad**: soportado en navegadores modernos.
- **Rendimiento**: actualización en tiempo real sin retrasos.
- **Estética**: Diseño simple y profesional.

---

## 4. Tecnologías Utilizadas
- **Frontend**: Angular 18+
- **Backend**: C# .NET 8+
- **Base de datos**: SQL Server 2022
- **Contenedores**: Docker

---

## 5. Arquitectura del Sistema
El sistema sigue una arquitectura cliente-servidor:

- **Frontend (Angular)**: interfaz gráfica para control y tablero.
- **Backend (C# .NET 8)**: API REST que gestiona lógica de negocio y comunicación con la base de datos.
- **Base de datos (SQL Server 2022 en Docker)**: persistencia de resultados, partidos, jugadores y estadísticas.
- **Comunicación en tiempo real**: implementada con servicios Angular para sincronizar control y display.


---

## 6. Desarrollo del Proyecto
1. **Diseño inicial** basado en requisitos de las instrucciones.
2. **Implementación del Frontend Angular**:
   - Componente control (botones para sumar/restar puntos, manejar tiempo, etc.).
   - Componente display (mostrar tablero con puntajes y cronómetro).
   - Servicio realtime.service.ts para mantener sincronización en tiempo real.
3. **Backend en C# .NET 8**:
   - Endpoints para manejar puntos, tiempo, cuartos y faltas.
   - Persistencia en SQL Server.
   - Sincronización para actualizaciones inmediatas.
4. **Base de datos en SQL Server 2022 (Docker)**:
   - Tabla general en la que se guarda la información: "game".

---

## 7. Pruebas Realizadas
- Prueba unitaria de suma/resta de puntos.
- Prueba de tiempo por cuarto.
- Prueba de reinicio general del marcador.
- Pruebas de usabilidad general y rendimiento.

---

## 8. Conclusiones
- Se implementó una solución completa y funcional que cumple con las reglas del baloncesto en cuanto a marcador y tiempos.
- La separación de controlador y tablero garantiza flexibilidad para distintos escenarios de uso.
- El uso de contenedores Docker asegura portabilidad y facilita despliegue en producción.
- Se logró sincronización en tiempo real entre control y tablero mediante servicios Angular y API en .NET 8.
- El sistema puede escalarse para incluir estadísticas de jugadores, reportes históricos y dashboards avanzados.

---

## 9. Integrantes
- Edrei Andrés Girón Leonardo / 7690-21-218
- Edward Alexander Aguilar Flores / 7690-21-7651
---
