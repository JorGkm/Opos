using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ExamenTestDiputacion
{
    public class Importador
    {
        public async Task<List<Pregunta>> CargarDesdeTexto(string rutaArchivo)
        {
            List<Pregunta> preguntas = new List<Pregunta>();

            if (!File.Exists(rutaArchivo))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[CRÍTICO] No existe el archivo: {rutaArchivo}");
                Console.ResetColor();
                return preguntas;
            }

            string contenidoTotal = File.ReadAllText(rutaArchivo, Encoding.UTF8);

            //Separamos las preguntas de las respuestas
            string[] partes = contenidoTotal.Split(new string[] { "### RESPUESTAS ###" }, StringSplitOptions.None);

            if (partes.Length < 2)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n[ERROR DE FORMATO] No encuentro '### RESPUESTAS ###'");
                Console.ResetColor();
            }

            //Guardamos por separado las preguntas y las respuestas
            string textoPreguntas = partes[0];
            string textoRespuestas = (partes.Length > 1) ? partes[1] : "";
            
            
            var lineas = textoPreguntas.Split('\n');
            Pregunta? preguntaActual = null;
            //Comprobamos los enunciados y las opciones posibles (respuestas)
            Regex regexInicio = new Regex(@"^\s*(\d+)[\.\-](?!\d)\s*(.*)");
            
            Regex regexOpcion = new Regex(@"^([a-d])\)\s*(.*)", RegexOptions.IgnoreCase);

            foreach (var l in lineas)
            {
                string linea = l.Trim();
                if (string.IsNullOrWhiteSpace(linea)) continue;
                
                var matchPregunta = regexInicio.Match(linea);
                var matchOpcion = regexOpcion.Match(linea);

                //Detección de otra pregunta nueva
                bool pareceNuevaPregunta = matchPregunta.Success;

                //En caso de que haya números en los enunciados y no sean nuevas preguntas
                if (pareceNuevaPregunta && preguntaActual != null && preguntaActual.Opciones.Count == 0)
                {
                    pareceNuevaPregunta = false; // Lo forzamos a ser texto normal
                }

                if (pareceNuevaPregunta)
                {
                    // Guardamos la anterior si estaba completa
                    if (preguntaActual != null && preguntaActual.Opciones.Count >= 2)
                        preguntas.Add(preguntaActual);

                    preguntaActual = new Pregunta
                    {
                        NumeroPregunta = int.Parse(matchPregunta.Groups[1].Value),
                        Enunciado = matchPregunta.Groups[2].Value
                    };
                }
                else if (matchOpcion.Success && preguntaActual != null)
                {
                    preguntaActual.Opciones.Add(matchOpcion.Groups[2].Value);
                }
                else if (preguntaActual != null)
                {
                    // Es texto continuado (del enunciado o de la última opción)
                    if (preguntaActual.Opciones.Count == 0) 
                    {
                        preguntaActual.Enunciado += " " + linea;
                    }
                    else if(preguntaActual.Opciones.Count < 5) 
                    {
                        // Añadimos a la última opción detectada
                        preguntaActual.Opciones[preguntaActual.Opciones.Count - 1] += " " + linea;
                    }
                }
            }
            // Añadir la última pregunta del fichero
            if (preguntaActual != null) preguntas.Add(preguntaActual);

            Console.WriteLine($" -> Se ha cargado el archivo: {rutaArchivo}");
            Console.WriteLine($" -> Preguntas leídas: {preguntas.Count}");

            // Guardamos las respuestas
            Dictionary<int, char> respuestas = new Dictionary<int, char>();

            // Limpieza básica
            textoRespuestas = textoRespuestas.Replace("PREG", "").Replace("RESP", "").Replace("Página", "");
            
            // Regex para tabla de respuestas
            Regex regexResp = new Regex(@"(\d+)[\s\r\n]+([A-D])", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            var coincidencias = regexResp.Matches(textoRespuestas);
            Console.WriteLine($" -> Respuestas detectadas en tabla: {coincidencias.Count}");

            foreach (Match m in coincidencias)
            {
                int num = int.Parse(m.Groups[1].Value);
                char letra = m.Groups[2].Value.ToUpper()[0];
                if (!respuestas.ContainsKey(num)) respuestas.Add(num, letra);
            }

            // Añadimos a cada pregunta su respuesta correcta
            int respuestasAsignadas = 0;
            foreach (var p in preguntas)
            {
                if (respuestas.ContainsKey(p.NumeroPregunta))
                {
                    p.RespuestaCorrecta = respuestas[p.NumeroPregunta];
                    respuestasAsignadas++;
                }
                else
                {
                    p.RespuestaCorrecta = '?';
                }
            }
            Console.WriteLine($" -> Respuestas vinculadas: {respuestasAsignadas}");
            return  preguntas;
        }
    }
}