using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YGOPRODeck_Reader
{
    public struct Card
    {
        public string name;
        public int[] id;
    }

    class Program
    {
        public static string[] files = {
                "291078.ydk",
                "289821.ydk",
                "282562.ydk",
                "291169.ydk",
                "285508.ydk",
                "280499.ydk",
                "280420.ydk",
                "285018.ydk",
                "281184.ydk",
                "286001.ydk",
                "292288.ydk",
                "286357.ydk",
                "291094.ydk" 
        };

        static void Main(string[] args)
        {
            int select;
            Database database = Database.Load("card_database.txt");
            LinkedList<int> list = Function.Count(files);

            string[] main = { 
                "What would you like to do?", 
                "0) Exit.", 
                "1) Output the entire card database.", 
                "2) Output the amount of all cards used in every deck in total.", 
                "3) Output all cards with unrecognized IDs." 
            };

            #region Main Menu
            do
            {
                Console.WriteLine("The following files are up for comparison:");
                for (int i = 0; i < files.Length; i++)
                {
                    Console.WriteLine(files[i]);
                }
                Console.WriteLine();
                for (int i = 0; i < main.Length; i++)
                {
                    Console.WriteLine(main[i]);
                }
                Console.WriteLine();
                Console.WriteLine("(Just write the number of the option you want to pick.)");
                Console.Write("Input: ");

                string input = Console.ReadLine();
                while ((!int.TryParse(input, out select)) && (select < main.Length - 1)) // Not working properly with letters. Check Option 2 and 3 as well.
                {
                    InvalidInput();
                }
                Console.WriteLine();

                switch (select)
                {
                    case 1:
                        Console.WriteLine("Card ID  - Card Name");
                        database.Write();
                        ProcessComplete();
                        break;
                    case 2:
                        Page.Option2();
                        ProcessComplete();
                        break;
                    case 3:
                        Console.WriteLine("Unrecognized IDs:");
                        foreach (var id in list)
                        {
                            Console.WriteLine(id);
                        }
                        ProcessComplete();
                        break;
                    default:
                        Console.WriteLine("Goodbye. Press any key to exit.");
                        break;
                }
            } while (select != 0);
            #endregion

            Console.ReadKey();
        }

        public static void ProcessComplete()
        {
            Console.WriteLine();
            Console.WriteLine("Process complete. Press any key to return to menu.");
            Console.ReadKey();
            Console.Clear();
        }

        public static void InvalidInput()
        {
            Console.SetCursorPosition(0, Console.CursorTop - 2);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.WriteLine($"(Invalid input. Try again.)");
        }
    }

    public class Page
    {
        public static void Option2()
        {
            Console.WriteLine("Do you want the list sorted by descending amount of cards? (Y/N)");
            Console.Write("Input: ");

            string input = Console.ReadLine();
            char select;
            while (!char.TryParse(input, out select) && ((char.ToUpper(select) != 'Y') || (char.ToUpper(select) != 'N')))
            {
                Program.InvalidInput();
            }
            Console.WriteLine();

            Console.WriteLine($"Out of {Program.files.Length} decks there were...");
            Console.WriteLine();
            Console.WriteLine("Amount - Name - Ratio per deck");

            switch (char.ToLower(select))
            {
                case 'y':
                    Function.SortAndWrite();
                    break;
                case 'n':
                    Function.WriteCount(0);
                    break;
                default:
                    break;
            }
        }
    }

    public class Database
    {
        #region Linked List
        public class Node : IComparable<Node>
        {
            public Card carddata;
            public int count;
            public Node next;
            public Node prev;

            public Node()
            {

            }

            public Node(Card carddata, int count)
            {
                this.carddata = carddata;
                this.count = count;
                this.next = null;
                this.prev = null;
            }

            public int CompareTo(Node other)
            {
                return -count.CompareTo(other.count);
            }

            public override string ToString()
            {
                return $"{count}x - {carddata.name} ({(float)count / Program.files.Length})";
            }
        }

        public static Node head;
        public static Node tail;

        
        public static int Count
        {
            get
            {
                int anz = 0;

                for (Node current = head; current != null; current = current.next)
                {
                    anz++;
                }

                return anz;
            }
        }
        
        public Database()
        {
            head = null;
            tail = null;
        }
        #endregion

        #region Functions
        public static Database Load(string filename)
        {
            Database database = new Database();

            using (StreamReader reader = new StreamReader(filename))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    if (line != "" && line[0] != '#')
                    {
                        string[] lines = line.Split('|');

                        Card card;
                        card.name = lines[0].Trim();
                        card.id = lines[1].Split(',').Select(int.Parse).ToArray();

                        database.AddLast(card, 0);
                    }
                }
            }

            return database;
        }

        public void AddLast(Card card, int count)
        {
            Node node = new Node(card, count);

            if (head == null)
            {
                head = tail = node;
                return;
            }

            node.prev = tail;
            tail.next = node;
            tail = node;
        }

        public void Write()
        {
            for (Node current = head; current != null; current = current.next)
            {
                Console.Write(current.carddata.id[0]);
                if (current.carddata.id[0].ToString().Length != 8)
                {
                    for (int i = 0; i < 8 - current.carddata.id[0].ToString().Length; i++)
                    {
                        Console.Write(' ');
                    }
                }
                Console.WriteLine($" - {current.carddata.name}");
            }
        }
        #endregion
    }

    public class Function
    {
        public static LinkedList<int> Count(string[] files)
        {
            LinkedList<int> list = new LinkedList<int>();

            for (int i1= 0; i1 < files.Length; i1++)
            {
                using (StreamReader reader = new StreamReader(files[i1]))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();

                        if (line != "" && line[0] != '#' && line[0] != '!')
                        {
                            for (Database.Node current = Database.head; current != null; current = current.next)
                            {
                                for (int i2 = 0; i2 < current.carddata.id.Length; i2++)
                                {
                                    if (current.carddata.id[i2] == int.Parse(line))
                                    {
                                        current.count++;
                                        goto label;
                                    }
                                    else if (current == Database.tail)
                                    {
                                        list.AddLast(int.Parse(line));
                                    }
                                }
                            }
                            label:;
                        }
                    }
                }
            }

            return list;
        }

        public static void WriteCount(int i)
        {
            for (Database.Node current = Database.head; current != null; current = current.next)
            {
                if (current.count > i)
                {
                    Console.WriteLine($"{current.count}x - {current.carddata.name}");
                }
            }
        }

        public static void SortAndWrite()
        {
            List<Database.Node> list = new List<Database.Node>();
            for (Database.Node current = Database.head; current != null; current = current.next)
            {
                list.Add(current);
            }
            list.Sort();

            foreach (var Node in list)
            {
                Console.WriteLine(Node);
            }
        }
    } // Essentially a pointless class, due to the fact you could move all the methods to the 'Database' class and save yourself the effort to make Nodes public and static, which basically also makes them pointless, but this looks cooler. And also makes me look stupider.
}
