using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enigma_Emulator {
    class EnigmaMachine {
        /* Enigma Machine
           Modelled after Enigma I, from ~1930
        */
        private Dictionary<Char, Char> plugBoard;

        // The machine has three rotors and a reflector
        private Rotor[] rotors;
        private Rotor reflector;

        private const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        // Rotor and reflectors. These configurations are constant and the same on every Enigma machine
        private const string rotorIconf = "EKMFLGDQVZNTOWYHXUSPAIBRCJ";
        private const string rotorIIconf = "AJDKSIRUXBLHWTMCQGZNPYFVOE";
        private const string rotorIIIconf = "BDFHJLCPRTXVZNYEIWGAKMUSQO";

        private const string reflectorAconf = "EJMZALYXVBWFCRQUONTSPIKHGD";
        private const string reflectorBconf = "YRUHQSLDPXNGOKMIEBFZCWVJAT";
        private const string reflectorCconf = "FVPJIAOYEDRZXWGCTKUQSBNMHL";

        // Rotor class representing one rotor
        private class Rotor {
            // The current char of the alphabet, and position of it. This char is visible outside the machine
            private int outerPosition;
            public char outerChar { get; set; }

            // The fixed alphabet of the rotor
            private string wiring;

            // turnOver is the notch on which letter the rotors turnover point is
            private char turnOver;

            public string name { get; }

            // Ring is the wiring setting relative to the turnover notch and position
            // Basically part of the initialization vector
            public char ring { get; set; }

            public int[] map { get; }
            public int[] revMap { get; }

            public Rotor(string w, char to, string n) {
                turnOver = to;
                outerPosition = 0;

                ring = 'A'; // A default ring setting
                name = n;

                map = new int[26];
                revMap = new int[26];

                setWiring(w);
            }

            public void setWiring(string newW) {
                wiring = newW;
                outerChar = wiring.ToCharArray()[outerPosition];

                // Fill the mapping arrays
                for (int i = 0; i < 26; i++) {
                    int match = ((int)wiring.ToCharArray()[i]) - 65;
                    map[i] = (26 + match - i) % 26;
                    revMap[match] = (26 + i - match) % 26;
                }
            }

            public void setOuterPosition(int i) {
                outerPosition = i;
                outerChar = alphabet.ToCharArray()[outerPosition];
            }

            public int getOuterPosition() {
                return outerPosition;
            }

            public void setOuterChar(char c) {
                outerChar = c;
                outerPosition = alphabet.IndexOf(outerChar);
            }

            public void step() {
                outerPosition = (outerPosition + 1) % 26;
                outerChar = alphabet.ToCharArray()[outerPosition];
            }

            public bool isInTurnOver() {
                return outerChar == turnOver;
            }
        }

        private void rotateRotors(Rotor[] r) {
            if (r.Length == 3) {
                if (r[1].isInTurnOver()) {
                    // If rotor II is on turnOver, all rotors step
                    r[0].step();
                    r[1].step();
                } else if (r[2].isInTurnOver()) {
                    // If rotor III is on turnOver, the two rotors to the right step
                    r[1].step();
                }

                // Rotor III always steps
                r[2].step();
            }
        }

        // Apply the rotor scramble to character using all three rotors
        // Argumentent reverse decides which direction we are scrambling
        private char rotorMap(char c, bool reverse) {
            int cPos = (int)c - 65;
            if (!reverse) {
                for (int i = rotors.Length - 1; i >= 0; i--) {
                    cPos = rotorValue(rotors[i], cPos, reverse);
                }
            } else {
                for (int i = 0; i < rotors.Length; i++) {
                    cPos = rotorValue(rotors[i], cPos, reverse);
                }
            }

            return alphabet.ToCharArray()[cPos];
        }

        private int rotorValue(Rotor r, int cPos, bool reverse) {
            int rPos = (int)r.ring - 65;
            int d;
            if (!reverse)
                d = r.map[(26 + cPos + r.getOuterPosition() - rPos) % 26];
            else
                d = r.revMap[(26 + cPos + r.getOuterPosition() - rPos) % 26];

            return (cPos + d) % 26;
        }

        // Apply the reflector, the part that comes after the rotors
        private char reflectorMap(char c) {
            int cPos = (int)c - 65;
            cPos = (cPos + reflector.map[cPos]) % 26;
            return alphabet.ToCharArray()[cPos];
        }

        // Constructor
        public EnigmaMachine() {
            plugBoard = new Dictionary<char, char>();

            // Notch and alphabet are fixed on the rotor
            // First argument is alphabet, second is the turnover notch
            Rotor rI = new Rotor(rotorIconf, 'Q', "I");
            Rotor rII = new Rotor(rotorIIconf, 'E', "II");
            Rotor rIII = new Rotor(rotorIIIconf, 'V', "III");
            rotors = new Rotor[] { rI, rII, rIII }; // Default ordering of rotors
            reflector = new Rotor(reflectorAconf, ' ', "");
        }

        public void setReflector(char conf) {
            if (conf != 'A' && conf != 'B' && conf != 'C') {
                throw new ArgumentException("Invalid argument");
            }

            string wiring = "";
            switch (conf) {
                case 'A':
                    wiring = reflectorAconf;
                    break;
                case 'B':
                    wiring = reflectorBconf;
                    break;
                case 'C':
                    wiring = reflectorCconf;
                    break;
            }
            reflector.setWiring(wiring);
        }

        // Enter the ring settings and initial rotor positions
        public void setSettings(char[] rings, char[] grund) {
            if (rings.Length != rotors.Length || grund.Length != rotors.Length) {
                throw new ArgumentException("Invalid argument lengths");
            }

            for (int i = 0; i < rotors.Length; i++) {
                rotors[i].ring = Char.ToUpper(rings[i]);
                rotors[i].setOuterChar(Char.ToUpper(grund[i]));
            }
        }

        public void setSettings(char[] rings, char[] grund, string rotorOrder) {
            Rotor rI = null;
            Rotor rII = null;
            Rotor rIII = null;

            // Get the current ordering
            for (int i = 0; i < rotors.Length; i++) {
                if (rotors[i].name == "I")
                    rI = rotors[i];
                if (rotors[i].name == "II")
                    rII = rotors[i];
                if (rotors[i].name == "III")
                    rIII = rotors[i];
            }

            string[] order = rotorOrder.Split('-');

            // Set the new ordering
            for (int i = 0; i < order.Length; i++) {
                if (order[i] == "I")
                    rotors[i] = rI;
                if (order[i] == "II")
                    rotors[i] = rII;
                if (order[i] == "III")
                    rotors[i] = rIII;
            }

            setSettings(rings, grund);
        }

        public void setSettings(char[] rings, char[] grund, string rotorOrder, char reflectorConf) {
            setReflector(reflectorConf);
            setSettings(rings, grund, rotorOrder);
        }

        // Encrypts or decrypts a message
        public string runEnigma(string msg) {
            StringBuilder encryptedMessage = new StringBuilder();

            msg = msg.ToUpper();

            foreach (char c in msg) {
                encryptedMessage.Append(encryptChar(c));
            }

            return encryptedMessage.ToString();
        }

        // Encrypts (or decrypts) a single character
        private char encryptChar(char c) {

            // Rotate the rotors before scrambling
            rotateRotors(rotors);

            // Into plugboard from keyboard <--
            if (plugBoard.ContainsKey(c)) {
                c = plugBoard[c];
            }

            // Scramble with rotors
            // First we go all the way through the rotors <--
            c = rotorMap(c, false);

            // Reflect at the end so we don't just unscramble it again when we go back
            // If the line below is commented out, the cipher will be equal to the message
            c = reflectorMap(c);

            // Go back through all the rotors the other way -->
            c = rotorMap(c, true);

            // Plugboard again, from other direction -->
            if (plugBoard.ContainsKey(c)) {
                c = plugBoard[c];
            }

            // Character is now encrypted
            return c;
        }

        // Add a character pair into the plugboard
        public void addPlug(char c, char cc) {
            if (Char.IsLetter(c) && Char.IsLetter(cc)) {
                c = Char.ToUpper(c);
                cc = Char.ToUpper(cc);
                if (c != cc && !plugBoard.ContainsKey(c)) {
                    plugBoard.Add(c, cc);
                    plugBoard.Add(cc, c);
                }
            } else {
                throw new ArgumentException("Invalid character");
            }
        }
    }
}
