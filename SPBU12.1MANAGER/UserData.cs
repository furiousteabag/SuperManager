using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.Serialization;

namespace SPBU12._1MANAGER {
    [Serializable]
    public class UserData {
        public Color color1, color2, fontColor;
        public Font fileFont, mainFont, dialogFont;
        
        public string login, password;

        public UserData() {
            color1 = Color.WhiteSmoke;
            color2 = Color.WhiteSmoke;
            fontColor = Color.Black;
            fileFont = new Font("Arial", 8f);
            mainFont = new Font("Arial", 8f);
            dialogFont = new Font("Arial", 8f);
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            SimpleEncryption se = new SimpleEncryption("kek");
            this.login = se.Encrypt(this.login);
            this.password = se.Encrypt(this.password);
            Console.WriteLine("OnSerializing fired.");
        }

        [OnDeserialized]
        private void OnDeserializing(StreamingContext context)
        {
            SimpleEncryption se = new SimpleEncryption("keek");
            this.login = se.Decrypt(this.login);
            this.password = se.Decrypt(this.password);
            Console.WriteLine("OnDeserializing fired.");
        }
    }

}
