using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Enigma_Emulator {
    class Program {
        static void Main(string[] args) {
            EnigmaMachine machine = new EnigmaMachine();
            EnigmaSettings eSettings = new EnigmaSettings();

            querySettings(eSettings);

            string message = "";
            Console.Write("Enter message to encrypt: ");
            message = Console.ReadLine();
            while (!Regex.IsMatch(message, @"^[a-zA-Z ]+$")) {
                Console.Write("Only letters A-Z is allowed, try again: ");
                message = Console.ReadLine();
            }
            message = message.Replace(" ", "").ToUpper();

            // Enter settings on machine
            machine.setSettings(eSettings.rings, eSettings.grund, eSettings.order, eSettings.reflector);

            // The plugboard settings
            foreach (string plug in eSettings.plugs) {
                char[] p = plug.ToCharArray();
                machine.addPlug(p[0], p[1]);
            }

            // Message encrypt
            Console.WriteLine();
            Console.WriteLine("Plain text:\t" + message);
            string enc = machine.runEnigma(message);
            Console.WriteLine("Encrypted:\t" + enc);

            // Reset the settings before decrypting!
            machine.setSettings(eSettings.rings, eSettings.grund, eSettings.order, eSettings.reflector);

            // Message decrypt
            string dec = machine.runEnigma(enc);
            Console.WriteLine("Decrypted:\t" + dec);
            Console.WriteLine();

            Console.ReadLine();
        }

        /* Query the user for settings
           Does not check for invalid input at the moment
        */
        private static void querySettings(EnigmaSettings e) {
            string r;
            Console.WriteLine("Enigma Machine Emulator\n");
            Console.Write("Do you want to: [1] Specify settings [2] Use default settings? (Default: [2]): ");
            r = Console.ReadLine();
            while (r != "1" && r != "2" && r != "") {
                Console.Write("Invalid input, enter 1, 2 or 3 ");
                r = Console.ReadLine();
            }
            if (r == "1") {
                Console.Write("Enter the ring settings (Ex. AAA, MCK, Default: AAA): ");
                r = Console.ReadLine();
                if (r == "")
                    e.rings = new char[] { 'A', 'A', 'A' };
                else
                    e.rings = r.ToCharArray();

                Console.Write("Enter the inital rotor start settings (Ex. AAA, MCK, Default: AAA): ");
                r = Console.ReadLine();
                if (r == "")
                    e.grund = new char[] { 'A', 'A', 'A' };
                else
                    e.grund = r.ToCharArray();

                Console.Write("Enter the order of the rotors (Ex. I-II-III, III-I-II, Default: I-II-III): ");
                r = Console.ReadLine();
                if (r == "")
                    e.order = "I-II-III";
                else
                    e.order = r;

                Console.Write("Enter the reflector to use (A, B, or C, Default: B): ");
                r = Console.ReadLine();
                if (r == "")
                    e.reflector = 'B';
                else
                    e.reflector = r.ToCharArray()[0];

                Console.Write("Enter the plugboard configuration (Ex. KH AB CE IJ, Default: None): ");
                r = Console.ReadLine();
                if (r == "")
                    e.plugs.Clear();
                else {
                    string[] plugs = r.Split(' ');
                    foreach (string s in plugs) {
                        e.plugs.Add(s);
                    }
                }

            } else if (r == "2" || r == "") {
                e.setDefault();
            }

            Console.WriteLine();
        }

        // Short class to hold the settings
        private class EnigmaSettings {
            public char[] rings { get; set; }
            public char[] grund { get; set; }
            public string order { get; set; }
            public char reflector { get; set; }
            public List<string> plugs = new List<string>();

            public EnigmaSettings() {
                setDefault();
            }

            public void setDefault() {
                rings = new char[] { 'A', 'A', 'A' };
                grund = new char[] { 'A', 'A', 'A' };
                order = "I-II-III";
                reflector = 'B';
                plugs.Clear();
            }
        }

    }
}
