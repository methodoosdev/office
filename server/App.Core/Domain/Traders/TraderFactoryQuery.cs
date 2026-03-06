using App.Core.Infrastructure;

namespace App.Core.Domain.Traders
{
    public abstract class TraderFactoryQuery
    {
        private readonly int _logistikiProgramTypeId;
        private readonly int _categoryBookTypeId;

        public TraderFactoryQuery(int logistikiProgramTypeId, int categoryBookTypeId)
        {
            _logistikiProgramTypeId = logistikiProgramTypeId;
            _categoryBookTypeId = categoryBookTypeId;
        }

        public abstract string SoftOne_C { get; }
        public abstract string SoftOne_B { get; }
        public abstract string Prosvasis { get; }
        public abstract string Prosvasis_C { get; }
        public string Get()
        {
            var logistikiProgramType = (LogistikiProgramType)_logistikiProgramTypeId;
            var categoryBookType = (CategoryBookType)_categoryBookTypeId;

            switch (logistikiProgramType)
            {
                case LogistikiProgramType.SoftOne:
                    switch (categoryBookType)
                    {
                        case CategoryBookType.C:
                            return SoftOne_C;

                        case CategoryBookType.B:
                            return SoftOne_B;

                        default:
                            throw new NopException($"Not supported category book: '{categoryBookType}'");
                    }

                case LogistikiProgramType.Prosvasis:
                    switch (categoryBookType)
                    {
                        case CategoryBookType.C:
                            return Prosvasis_C;

                        case CategoryBookType.B:
                            return Prosvasis;

                        default:
                            throw new NopException($"Not supported category book: '{categoryBookType}'");
                    }

                default:
                    throw new NopException($"Not supported programm type name: '{logistikiProgramType}'");
            }
        }

    }
}
