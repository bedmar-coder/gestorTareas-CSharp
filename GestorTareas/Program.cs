namespace GestorTareas
{

    //creación de la excepción propia
    public class TareaException : Exception
    {
        //como tendremos dos tipos pues cada uno tendrá que escribir su mensaje
        public TareaException(string message) : base(message) { }
    }



    class Program


    {
        // usamos path combine apra generar la carpeta y el fichero sin importar los separadores dels sitema
        static string carpetaDatos = "datos";
        static string rutaArchivo = Path.Combine(carpetaDatos, "tareas.txt");

        private static string PENDIENTE = "[ ] ";
        private static string HECHA = "[X] ";

        static void Main(string[] args)
        {
            //semaforo para ver cuandos alir del menu
            bool semaforo = true;
            //en la siguiente guardamos las tareas
            List<string> tareas = new List<string>();

            //try principal que protege la carga de archivos y pone el finally
            try
            {

                CargarTareas(tareas);

                while (semaforo)
                {
                    MostrarMenu();

                    try
                    {
                        //opción del usuario parseandola
                        int opcion = int.Parse(Console.ReadLine());

                        Console.WriteLine();

                        //switch con las decisiones
                        switch (opcion)
                        {
                            case 1:
                                AnadirTarea(tareas);
                                break;

                            case 2:
                                MostrarTareas(tareas);
                                break;

                            case 3:
                                Console.Write("Introduce la palabra o texto con el que buscar: ");
                                string filtro = Console.ReadLine();
                                MostrarTareas(tareas, filtro);
                                break;

                            case 4:
                                MarcarCompletada(tareas);
                                break;

                            case 5:
                                EliminarTarea(tareas);
                                break;

                            case 6:
                                VerEstadisticas(tareas);
                                break;

                            //con el 0 salimos
                            case 0:
                                Console.WriteLine("Saliendo...");
                                GuardarTareas(tareas);
                                semaforo = false;
                                break;

                            //controlamos el ddefault
                            default:
                                Console.WriteLine("Opción no válida elige un número del 0 al 6");
                                break;
                        }
                    }
                    catch (TareaException e)
                    {
                        // primero lo más especifico que es nuestra propia excepción
                        Console.WriteLine("\n" + e.Message);
                    }
                    catch (FormatException)
                    {
                        // aqui si pone letras
                        Console.WriteLine("\nNo has introducido un numero");
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // esta por si pone una tarea fuera de la seleccion
                        Console.WriteLine("\nLa tarea seleccionado no existe");
                    }
                    catch (Exception e)
                    {
                        // captura generica
                        Console.WriteLine(e.Message);
                    }


                    //este trozo de codigo para limpiar pantalla
                    if (semaforo)
                    {
                        Console.WriteLine("\nPulsa cualquier tecla para continuar");
                        Console.ReadKey();
                        Console.Clear();
                    }
                }
            }
            catch (Exception e)
            {
                //cogemops posibles excepciones con el fichero
                Console.WriteLine($"\nNo se ha podido iniciar o guardar: {e.Message}");
            }
            finally
            {
                // esto cierra la aplicación ya sea que acabe bien o mal
                Console.WriteLine("\nAplicación cerrada. Sesión finalizada correctamente");
            }
        }

            //metodo que umprime el menu
            static void MostrarMenu()
            {
                string SEPARADOR_MENU = "=====================================";

                Console.WriteLine(SEPARADOR_MENU);
                Console.WriteLine("            GESTOR DE TAREAS         ");
                Console.WriteLine(SEPARADOR_MENU);
                Console.WriteLine("1. Añadir tarea");
                Console.WriteLine("2. Listar tareas");
                Console.WriteLine("3. Buscar tarea");
                Console.WriteLine("4. Marcar como completada");
                Console.WriteLine("5. Eliminar tarea");
                Console.WriteLine("6. Ver estadísticas");
                Console.WriteLine("0. Salir y guardar");
                Console.WriteLine(SEPARADOR_MENU);
                Console.Write("Elige una opcion: ");
            }
        

        //metodo para añadir tarea 
        static void AnadirTarea(List<string> tareas)
        {
            Console.Write("Escribe la descripcion de la tarea que quieres crear: ");
            string descripcion = Console.ReadLine();

            //comprobamos escriba algo con este metodo propio de C# y si no lanzamos nuestra excepción
            if (string.IsNullOrEmpty(descripcion))
            {
                throw new TareaException("No has escrito nada así que no se ha guardado");
                
            }
            //creamos el string añadiendole por defecto sin hacer al inicio
            string nuevaTarea = PENDIENTE + descripcion;
            tareas.Add(nuevaTarea);
            Console.WriteLine("Tarea añadida");

        }


        //primer metodo de mostrar tareas
        static void MostrarTareas(List<string> tareas)

        {
            string titulo = "***Tus Tareas***";
            Console.WriteLine(titulo);

            //comprobamos la lista se amayor de 0
            if (tareas.Count == 0)
            {
                //si no lo es informamos y salimos
                Console.WriteLine("No hay tareas todavia");
                return;
            }

            //si las hay las recorremos

            for (int i = 0; i < tareas.Count; i++)
            {
                Console.WriteLine((i+1)+". " + tareas[i]);
            }
        }

        //metodo sobrecargado para filtrar
        static void MostrarTareas(List<string> tareas, string filtro)
        {
            Console.WriteLine("Resultados encontrados: ");
           
            //booleano para ver si hay coincidencias
            bool encontradas = false;

            //for para recorrer las tareas
            for (int i = 0; i < tareas.Count; i++)
            {
                // Comprobamos si la tarea contiene el filtro usandoignorecase para ignorar las mayusculas
                if (tareas[i].Contains(filtro, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine((i + 1) + ". " + tareas[i]);
                    //ponemos el bool en true
                    encontradas = true;
                }
            }

            //si el booleano seguia en false informamos no hemos encontrado coincidencias
            if (!encontradas)
            {
                Console.WriteLine("No se encontraron tareas que coincidan con tu busqueda");
            }
        }

        static void MarcarCompletada(List<string> tareas)
        {
            //de nuevo compruebo haya tareas
            if (tareas.Count == 0)
            {
                //y si no lanzamos nuestra exscepción
                throw new TareaException("No hay tareas disponibles");
                
            }
            //uso mi metodo anterior para enseñar las que hay y pueda elegir cualquiere poner como hecha
            MostrarTareas(tareas);
            Console.Write("Introduce el número de la tarea que has terminado: ");

            //parseamos la respusta y la guardamos
            int indice = int.Parse(Console.ReadLine());
            //si el número es incorrecto lanzamos la excepcion que indicaba el enunciado
            if (indice < 1 || indice > tareas.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

                //le restamos uno a su elección por como funcionan los indices
                int indiceReal = indice - 1;

                // verificamos si ya esta hecha
                if (tareas[indiceReal].StartsWith(HECHA))
                {
                    Console.WriteLine("Esta tarea ya estaba completada");
                }
                else
                {
                    // sustituimos solamente [ ] por [x]
                    string descripcion = tareas[indiceReal].Substring(4);
                    tareas[indiceReal] = HECHA + descripcion;

                    // generamos mensaje aleatorio con Random
                    //primero guardamos nuestros mensajes en un array 
                    string[] mensajesMotivacionales = {
                        "Ole! Te esfuerzas mucho",
                        "Un pasito más cerca de tus sueños ^^",
                        "Eres increible!",
                        "Cada día mas imaprableee",
                        "Muy bien! sigue asi :D"
                    };

                    //creamos un nuevo objeto random
                    Random random = new Random();
                    //elejimos un indice usando el length  dela rray de los mensajes
                    int indiceMensaje = random.Next(mensajesMotivacionales.Length);

                    Console.WriteLine("\nTarea completada y guardada");
                    Console.WriteLine(mensajesMotivacionales[indiceMensaje]);
                }
            }
            

        //este metodo lo usaré apra eliminar una tarea
        static void EliminarTarea(List<string> tareas)
        {
            //de nuevo compruebo el size
            if (tareas.Count == 0)
            {
                //si es cero mandamos la excepcion
                throw new TareaException("No hay tareas");
            }

            //de nuevo como antes mostramos las que hay para poder elegirlas
            MostrarTareas(tareas);
            //informamos
            Console.Write("Introduce el numero de la tarea que quieres borrar: ");

            //igual que antes guardamos la elección
            int indice = int.Parse(Console.ReadLine());
            //y lanzamos excepción si selige mal
            if (indice < 1 || indice > tareas.Count)
            {
                throw new ArgumentOutOfRangeException();
            }
            //borro de la lista poniendo el indice que es -1 por el tema de los indices reales
            tareas.RemoveAt(indice - 1);
            //informo
            Console.WriteLine("La tarrea ha sido eliminada");
        }

        //metodo para importar tareas desde persistencia
        static void CargarTareas(List<string> tareas)
        {
            //siguiendo el reto extrasi la carpeta no existe la creamos
            if (!Directory.Exists(carpetaDatos))
            {
                Directory.CreateDirectory(carpetaDatos);
            }

            //y si el archivo ya existe se lee y añade a tareas
            if (File.Exists(rutaArchivo))
            {
                string[] lineas = File.ReadAllLines(rutaArchivo);
                tareas.AddRange(lineas);
            }
        }

        //con este metodo guardamos la lista ene l fichero
        static void GuardarTareas(List<string> tareas)
        {
            
            File.WriteAllLines(rutaArchivo, tareas);
        }


        static void VerEstadisticas(List<string> tareas)
        {
           //apuntamos el total de tareas
            int total = tareas.Count;
            //como en todos los anteriores miro si hay 0
            if (total == 0)
            {
                //infomo si hay 0
                Console.WriteLine("No puedes calcualr estadisticas porque no hay tareas");
                //y tras eso salgo
                return;
            }
            //inicio una avriable de completadas a 0
            int completadas = 0;
            for (int i = 0; i < tareas.Count; i++)
            {
                //recorro las tareas y compruebo si empiezan por hecho
                if (tareas[i].StartsWith(HECHA))
                {
                    //cada vez encuentre una sumo 1 a completadas
                    completadas++;
                }
            }
            //las que tengo por hacer son la resta de las ehchas al total
            int porHacer = total - completadas;
            // calculamos el porcentaje redodneando con round y forzando con double
            double porcentaje = Math.Round((double)completadas / total * 100, 2);

            Console.WriteLine("***Estadisticas***");
            Console.WriteLine("Total de tareas: " + total);
            Console.WriteLine("Completadas:" + completadas);
            Console.WriteLine("Por hacer:" + porHacer);
            Console.WriteLine("Porcentaje de realizadas:" + porcentaje + "%");
        }
    }




}