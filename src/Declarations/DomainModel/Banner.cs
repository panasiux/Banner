using System;
using Declarations.Interfaces;

namespace Declarations.DomainModel
{
    public class Banner : IBanner
    {
        private DateTime _created;
        private DateTime? _modified;

        public int Id { get; set; }
        public string Html { get; set; }

        
        public DateTime Created
        {
            get => _created;
            set => _created = value.ToUniversalTime();
        }
        public DateTime? Modified
        {
            get => _modified;
            set
            {
                if (!value.HasValue)
                {
                    _modified = null;
                    return;
                }

                _modified = value.Value.ToUniversalTime();
            }
        }
    }
}
