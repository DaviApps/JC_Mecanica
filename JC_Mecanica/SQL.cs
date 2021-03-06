﻿using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JC_Mecanica {
    class SQL {
        SqlCeConnection connection;
        String table = "";

        String id = "";
        String _id = "";

        public SQL(String table) {
            This(Properties.Settings.Default.DataConnectionString, table);
        }

        public SQL(String DB, String table) {
            This(DB, table);
        }

        private void This(String DB, String table) {
            connection = new SqlCeConnection(DB);
            this.table = table;
        }

        public SQL setID(String ident) {
            this._id = ident;
            return this;
        }

        public SQL setID(String ident, String id) {
            this._id = ident; this.id = id;
            return this;
        }

        public SQL setID(String ident, int id) {
            this._id = ident; this.id = "" + id;
            return this;
        }

        public SQL setDynamic(String indent, String prop, String findValue) {
            connection.Open();

            SqlCeCommand cmd = new SqlCeCommand("SELECT * FROM " + table + " WHERE " + prop + " Like ?", connection);
            cmd.Parameters.AddWithValue("@p1", findValue);
            SqlCeDataReader re = cmd.ExecuteReader();

            MessageBox.Show(re ["id"].ToString());

            if (re.Read())
                this.id = re [indent].ToString();

            this._id = indent;
            re.Close();
            connection.Close();
            return this;
        }

        public bool isError() {
            return (_id.Equals("") || id.Equals("") || id.Equals("0"));
        }

        public bool exists() {
            SqlCeConnection connection = new SqlCeConnection(Properties.Settings.Default.DataConnectionString);
            connection.Open();

            int id_count = 0;

            if (!isError()) {
                SqlCeCommand cmd_count = new SqlCeCommand("SELECT COUNT(*) FROM [" + table + "] WHERE ([id] = @id)", connection);
                cmd_count.Parameters.AddWithValue("@id", _id);

                id_count = (int) cmd_count.ExecuteScalar();

                connection.Close();
            }

            return id_count > 0;
        }

        public String get(String item, String id) {
            this.id = id;
            return get(item);
        }

        public String get(String item) {
            String output = "";

            if (_id.Equals("") || _id.Equals("0") || id.Equals("")) return "ERROR";

            connection.Open();

            SqlCeCommand cmd = new SqlCeCommand("SELECT * FROM " + table + " WHERE " + _id + " = @id", connection);
            cmd.Parameters.AddWithValue("@id", id);
            SqlCeDataReader re = cmd.ExecuteReader();

            if (re.Read())
                output = re [item].ToString();

            re.Close();
            connection.Close();

            return output;
        }

        public void set(String item, String value, String id) {
            this.id = id;
            this.set(item, value);
        }

        public void set(String item, String value) {
            SqlCeConnection connection = new SqlCeConnection(Properties.Settings.Default.DataConnectionString);
            connection.Open();

            if (_id.Equals("") || _id.Equals("0") || id.Equals("") || _id.Equals("0")) return;

            SqlCeCommand cmd = new SqlCeCommand("UPDATE " + table + " SET " + item + " = @" + item + " WHERE " + _id + " = @id", connection);
            cmd.Parameters.AddWithValue(item, value);
            cmd.Parameters.AddWithValue("@id", id);

            SqlCeDataReader re = cmd.ExecuteReader();
            connection.Close();
        }

        public void remove() {
            SqlCeConnection connection = new SqlCeConnection(Properties.Settings.Default.DataConnectionString);
            connection.Open();

            SqlCeCommand cmd = new SqlCeCommand("DELETE FROM " + table + "  WHERE " + _id + " = @id", connection);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();

            connection.Close();
        }
    }
}
