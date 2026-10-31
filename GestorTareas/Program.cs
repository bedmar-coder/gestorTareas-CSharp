﻿namespace GestorTareas
{
    /// <summary>
    /// Custom exception thrown when a task operation can't be completed
    /// (for example, when the list is empty or the description is blank).
    /// </summary>
    /// @param message the text explaining what went wrong
    public class TaskException : Exception
    {
        // There are two cases that throw it, so each one passes its own message
        public TaskException(string message) : base(message) { }
    }


    class Program
    {
        // Path.Combine builds the folder and file path regardless of the system's separators.
        // The folder/file names are kept as they are so existing saved data is still found.
        static string dataFolder = "datos";
        static string filePath = Path.Combine(dataFolder, "tareas.txt");

        private static string PENDING = "[ ] ";
        private static string DONE = "[X] ";

        /// <summary>
        /// Entry point: loads the saved tasks and keeps showing the menu until the user exits.
        /// </summary>
        /// @param args command-line arguments (not used)
        static void Main(string[] args)
        {
            // Flag that tells us when to leave the menu
            bool running = true;
            // The tasks are stored in this list
            List<string> tasks = new List<string>();

            // Main try that protects the file loading and holds the finally block
            try
            {
                LoadTasks(tasks);

                while (running)
                {
                    ShowMenu();

                    try
                    {
                        // Parse the user's option
                        int option = int.Parse(Console.ReadLine());

                        Console.WriteLine();

                        // Switch with every menu choice
                        switch (option)
                        {
                            case 1:
                                AddTask(tasks);
                                break;

                            case 2:
                                ShowTasks(tasks);
                                break;

                            case 3:
                                Console.Write("Enter the word or text to search for: ");
                                string filter = Console.ReadLine();
                                ShowTasks(tasks, filter);
                                break;

                            case 4:
                                MarkAsCompleted(tasks);
                                break;

                            case 5:
                                DeleteTask(tasks);
                                break;

                            case 6:
                                ShowStatistics(tasks);
                                break;

                            // 0 exits the program
                            case 0:
                                Console.WriteLine("Exiting...");
                                SaveTasks(tasks);
                                running = false;
                                break;

                            // Handle any other value
                            default:
                                Console.WriteLine("Invalid option, choose a number from 0 to 6");
                                break;
                        }
                    }
                    catch (TaskException e)
                    {
                        // Most specific first: our own exception
                        Console.WriteLine("\n" + e.Message);
                    }
                    catch (FormatException)
                    {
                        // The user typed letters instead of a number
                        Console.WriteLine("\nYou didn't enter a number");
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // The user picked a task that doesn't exist
                        Console.WriteLine("\nThe selected task doesn't exist");
                    }
                    catch (Exception e)
                    {
                        // Generic catch
                        Console.WriteLine(e.Message);
                    }


                    // Clear the screen before showing the menu again
                    if (running)
                    {
                        Console.WriteLine("\nPress any key to continue");
                        Console.ReadKey();
                        Console.Clear();
                    }
                }
            }
            catch (Exception e)
            {
                // Catch possible problems with the file
                Console.WriteLine($"\nCouldn't start or save: {e.Message}");
            }
            finally
            {
                // This runs whether the app ends well or badly
                Console.WriteLine("\nApplication closed. Session ended successfully");
            }
        }

        /// <summary>
        /// Prints the main menu.
        /// </summary>
        static void ShowMenu()
        {
            string MENU_SEPARATOR = "=====================================";

            Console.WriteLine(MENU_SEPARATOR);
            Console.WriteLine("             TASK MANAGER            ");
            Console.WriteLine(MENU_SEPARATOR);
            Console.WriteLine("1. Add task");
            Console.WriteLine("2. List tasks");
            Console.WriteLine("3. Search task");
            Console.WriteLine("4. Mark as completed");
            Console.WriteLine("5. Delete task");
            Console.WriteLine("6. View statistics");
            Console.WriteLine("0. Save and exit");
            Console.WriteLine(MENU_SEPARATOR);
            Console.Write("Choose an option: ");
        }


        /// <summary>
        /// Asks the user for a description and adds it to the list as a pending task.
        /// </summary>
        /// @param tasks the list the new task is added to
        static void AddTask(List<string> tasks)
        {
            Console.Write("Write the description of the task you want to create: ");
            string description = Console.ReadLine();

            // Check something was written using this built-in C# method; if not, throw our exception
            if (string.IsNullOrEmpty(description))
            {
                throw new TaskException("You didn't write anything, so nothing was saved");
            }

            // Build the string, marking it as not done by default
            string newTask = PENDING + description;
            tasks.Add(newTask);
            Console.WriteLine("Task added");
        }


        /// <summary>
        /// Prints every task with its number.
        /// </summary>
        /// @param tasks the list of tasks to show
        static void ShowTasks(List<string> tasks)
        {
            string title = "***Your Tasks***";
            Console.WriteLine(title);

            // Check the list isn't empty
            if (tasks.Count == 0)
            {
                // If it is, tell the user and leave
                Console.WriteLine("There are no tasks yet");
                return;
            }

            // If there are tasks, go through them
            for (int i = 0; i < tasks.Count; i++)
            {
                Console.WriteLine((i + 1) + ". " + tasks[i]);
            }
        }

        /// <summary>
        /// Overloaded version that only prints the tasks containing the given text.
        /// </summary>
        /// @param tasks the list of tasks to search in
        /// @param filter the text to look for (case-insensitive)
        static void ShowTasks(List<string> tasks, string filter)
        {
            Console.WriteLine("Results found: ");

            // Flag to know whether there were any matches
            bool found = false;

            // Loop through the tasks
            for (int i = 0; i < tasks.Count; i++)
            {
                // Check if the task contains the filter, using IgnoreCase to ignore capital letters
                if (tasks[i].Contains(filter, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine((i + 1) + ". " + tasks[i]);
                    // Set the flag to true
                    found = true;
                }
            }

            // If the flag is still false, tell the user nothing matched
            if (!found)
            {
                Console.WriteLine("No tasks matched your search");
            }
        }

        /// <summary>
        /// Lets the user pick a task and marks it as done, showing a random motivational message.
        /// </summary>
        /// @param tasks the list of tasks to choose from
        static void MarkAsCompleted(List<string> tasks)
        {
            // Again, check there are tasks
            if (tasks.Count == 0)
            {
                // If not, throw our exception
                throw new TaskException("There are no tasks available");
            }

            // Reuse the previous method to show the tasks so the user can pick one to mark as done
            ShowTasks(tasks);
            Console.Write("Enter the number of the task you've finished: ");

            // Parse the answer and store it
            int index = int.Parse(Console.ReadLine());
            // If the number is wrong, throw the exception the assignment asked for
            if (index < 1 || index > tasks.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            // Subtract one from the choice because of how indexes work
            int realIndex = index - 1;

            // Check whether it's already done
            if (tasks[realIndex].StartsWith(DONE))
            {
                Console.WriteLine("This task was already completed");
            }
            else
            {
                // Replace only "[ ]" with "[X]"
                string description = tasks[realIndex].Substring(4);
                tasks[realIndex] = DONE + description;

                // Pick a random message with Random
                // First store our messages in an array
                string[] motivationalMessages = {
                    "Wow! You work really hard",
                    "One small step closer to your dreams ^^",
                    "You're amazing!",
                    "More unstoppable every daaay",
                    "Well done! Keep it up :D"
                };

                // Create a new Random object
                Random random = new Random();
                // Pick an index using the length of the messages array
                int messageIndex = random.Next(motivationalMessages.Length);

                Console.WriteLine("\nTask completed and saved");
                Console.WriteLine(motivationalMessages[messageIndex]);
            }
        }


        /// <summary>
        /// Lets the user pick a task and removes it from the list.
        /// </summary>
        /// @param tasks the list of tasks to delete from
        static void DeleteTask(List<string> tasks)
        {
            // Again, check the size
            if (tasks.Count == 0)
            {
                // If it's zero, throw the exception
                throw new TaskException("There are no tasks");
            }

            // Like before, show the existing tasks so the user can choose
            ShowTasks(tasks);
            // Tell the user what to do
            Console.Write("Enter the number of the task you want to delete: ");

            // Same as before, store the choice
            int index = int.Parse(Console.ReadLine());
            // And throw an exception if the choice is wrong
            if (index < 1 || index > tasks.Count)
            {
                throw new ArgumentOutOfRangeException();
            }
            // Remove it from the list using index - 1 to get the real index
            tasks.RemoveAt(index - 1);
            // Tell the user
            Console.WriteLine("The task has been deleted");
        }

        /// <summary>
        /// Loads the saved tasks from the file, creating the data folder if it doesn't exist.
        /// </summary>
        /// @param tasks the list the loaded tasks are added to
        static void LoadTasks(List<string> tasks)
        {
            // Following the extra challenge: if the folder doesn't exist, create it
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            // If the file already exists, read it and add its lines to the tasks
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                tasks.AddRange(lines);
            }
        }

        /// <summary>
        /// Saves the task list to the file, one task per line.
        /// </summary>
        /// @param tasks the list of tasks to save
        static void SaveTasks(List<string> tasks)
        {
            File.WriteAllLines(filePath, tasks);
        }


        /// <summary>
        /// Prints how many tasks there are, how many are done and the completion percentage.
        /// </summary>
        /// @param tasks the list of tasks to calculate the statistics from
        static void ShowStatistics(List<string> tasks)
        {
            // Store the total number of tasks
            int total = tasks.Count;
            // Like in the previous methods, check whether there are 0
            if (total == 0)
            {
                // Tell the user there are none
                Console.WriteLine("You can't calculate statistics because there are no tasks");
                // And leave
                return;
            }
            // Start a completed counter at 0
            int completed = 0;
            for (int i = 0; i < tasks.Count; i++)
            {
                // Go through the tasks and check whether they start with the "done" mark
                if (tasks[i].StartsWith(DONE))
                {
                    // Every time one is found, add 1 to completed
                    completed++;
                }
            }
            // The pending ones are the total minus the completed ones
            int pending = total - completed;
            // Calculate the percentage, rounding with Round and casting to double
            double percentage = Math.Round((double)completed / total * 100, 2);

            Console.WriteLine("***Statistics***");
            Console.WriteLine("Total tasks: " + total);
            Console.WriteLine("Completed:" + completed);
            Console.WriteLine("Pending:" + pending);
            Console.WriteLine("Completion percentage:" + percentage + "%");
        }
    }
}
