using System.Collections.Generic;
using System.Linq;

namespace App.Services.Customers
{
    public class CustomerActivityResult
    {
        private readonly List<string> _errors;
        private readonly List<string> _infos;
        private readonly List<string> _success;
        private readonly string _errorTitle = "Σφάλματα";
        private readonly string _infoTitle = "Αναφορά σφαλμάτων";
        private readonly string _successTitle = "Επιτυχημένες ενέργειες";

        public CustomerActivityResult()
        {
            _errors = new List<string>();
            _infos = new List<string>();
            _success = new List<string>();
        }

        public bool Success => !_errors.Any();

        public void AddError(string text, bool tab = false)
        {
            text = text.Replace("\n", "");
            _errors.Add(tab ? $"\t{text}\n" : $"{text}\n");
        }

        public void AddInfos(string text, bool tab = false)
        {
            text = text.Replace("\n", "");
            _infos.Add(tab ? $"\t{text}\n" : $"{text}\n");
        }

        public void AddSuccess(string text, bool tab = false)
        {
            text = text.Replace("\n", "");
            _success.Add(tab ? $"\t{text}\n" : $"{text}\n");
        }

        public void AddRange(CustomerActivityResult act)
        {
            _errors.AddRange(act._errors);
            _infos.AddRange(act._infos);
            _success.AddRange(act._success);
        }

        public override string ToString()
        {
            var value = string.Empty;

            if (_errors.Count > 0)
            {
                _errors.Insert(0, $"<b> {_errorTitle} </b>\n");
                _errors.Add("\n");
            }

            value += string.Join(' ', _errors);

            if (_infos.Count > 0)
            {
                _infos.Insert(0, $"<b> {_infoTitle} </b>\n");
                _infos.Add("\n");
            }

            value += string.Join(' ', _infos);

            if (_success.Count > 0)
            {
                _success.Insert(0, $"<b> {_successTitle} </b>\n");
                _success.Add("\n");
            }

            value += string.Join(' ', _success);

            //value += $"Δημιουργήθηκαν {_success.Count()} από {_errors.Count() + _success.Count()}.";

            return value;
        }
    }
}
