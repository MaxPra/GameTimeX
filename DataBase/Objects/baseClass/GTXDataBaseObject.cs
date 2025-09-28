namespace GameTimeX.DataBase.Objects.baseClass
{
    public abstract class GTXDataBaseObject
    {
        public abstract string TableName { get; }

        private string _tableName = string.Empty;

        /// <summary>
        /// Liefert den Tabellennamen in der GTX-Datenbank
        /// </summary>
        /// <returns></returns>
        public string GetTableName()
        {
            return _tableName;
        }

        protected void SetTableName(string tableName)
        {
            this._tableName = tableName;
        }
    }
}
