using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Opos
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

            string contenidoTotal = await File.ReadAllTextAsync(rutaArchivo, Encoding.UTF8);

            string[] partes = contenidoTotal.Split(new string[] { "### RESPUESTAS ###" }, StringSplitOptions.None);

            if (partes.Length < 2)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n[ERROR DE FORMATO] No encuentro '### RESPUESTAS ###'");
                Console.ResetColor();
            }

            string textoPreguntas = partes[0];
            string textoRespuestas = (partes.Length > 1) ? partes[1] : "";


            var lineas = textoPreguntas.Split('\n');
            Pregunta? preguntaActual = null;
            string temaActual = "General";
            string? nombreTemaActual = null;

            Regex regexInicio = new Regex(@"^\s*(\d+)[\.\-](?!\d)\s*(.*)");
            Regex regexOpcion = new Regex(@"^([a-d])\)\s*(.*)", RegexOptions.IgnoreCase);
            Regex regexTema = new Regex(@"^TEMA\s+(\d+)\s*[-:—–]?\s*(.*)", RegexOptions.IgnoreCase);

            foreach (var l in lineas)
            {
                string linea = l.Trim();
                if (string.IsNullOrWhiteSpace(linea)) continue;

                var matchTema = regexTema.Match(linea);
                if (matchTema.Success)
                {
                    temaActual = "Tema " + matchTema.Groups[1].Value;
                    nombreTemaActual = matchTema.Groups[2].Value.Trim();
                    if (string.IsNullOrEmpty(nombreTemaActual)) nombreTemaActual = null;
                    continue; 
                }

                var matchPregunta = regexInicio.Match(linea);
                var matchOpcion = regexOpcion.Match(linea);

                if (matchPregunta.Success)
                {
                    if (preguntaActual != null && preguntaActual.Opciones.Count >= 2)
                        preguntas.Add(preguntaActual);

                    preguntaActual = new Pregunta
                    {
                        NumeroPregunta = int.Parse(matchPregunta.Groups[1].Value),
                        Enunciado = matchPregunta.Groups[2].Value,
                        Tema = temaActual,
                        NombreTema = nombreTemaActual
                    };
                }
                else if (matchOpcion.Success && preguntaActual != null)
                {
                    preguntaActual.Opciones.Add(matchOpcion.Groups[2].Value);
                }
                else if (preguntaActual != null)
                {
                    if (preguntaActual.Opciones.Count == 0)
                        preguntaActual.Enunciado += " " + linea;
                    else
                        preguntaActual.Opciones[preguntaActual.Opciones.Count - 1] += " " + linea;
                }
            }
            if (preguntaActual != null) preguntas.Add(preguntaActual);

            Console.WriteLine($"\n -> Se ha cargado el archivo: {rutaArchivo}");
            Console.WriteLine($" -> Preguntas leídas: {preguntas.Count}");

            Dictionary<int, char> respuestas = new Dictionary<int, char>();

            textoRespuestas = textoRespuestas.Replace("PREG", "").Replace("RESP", "").Replace("Página", "");

            Regex regexResp = new Regex(@"(\d+)[\s\r\n]+([A-D])", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            var coincidencias = regexResp.Matches(textoRespuestas);
            Console.WriteLine($" -> Respuestas detectadas en tabla: {coincidencias.Count}");

            foreach (Match m in coincidencias)
            {
                int num = int.Parse(m.Groups[1].Value);
                char letra = m.Groups[2].Value.ToUpper()[0];
                if (!respuestas.ContainsKey(num)) respuestas.Add(num, letra);
            }

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
            return preguntas;
        }
    }
}
