using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;

namespace ClassLibrary
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string connType;
            string connServer;
            string connDb;
            string connUser;
            string connPass;
            var cmd = new RootCommand
            {
                new Option<string>(name: "--conn-type", description: "This accepts win or sql"){ IsRequired = true},
                new Option<string>(name: "--conn-server", description: "The SQL Server Name"){ IsRequired = true},
                new Option<string>(name: "--conn-db", description: "The SQL Database Name"){ IsRequired = true},
                new Option<string>(name: "--conn-user", description: "The SQL Auth User Name", getDefaultValue: () => "sa"),
                new Option<string>(name: "--conn-pass", description: "The SQL Auth User Pass"),
                new Option<string>(name: "--sqlite-path", description: "The path for the sqlite database to be created. Note this must be fully qualified. or it will create it where the bin is"){ IsRequired = true}

            };

            cmd.Handler = CommandHandler.Create<string, string, string, string?, string?, string>((connType, connServer, connDb, connUser, connPass, sqlitePath) =>
            {
                string sqlConnString = "";
                string dirPath = Path.GetDirectoryName(sqlitePath);
                string fileName = Path.GetFileName(sqlitePath);
                DirectoryInfo di = System.IO.Directory.CreateDirectory(dirPath);
                if (di == null)
                {
                    Console.WriteLine("Sorry but can't create your file in that dir");
                    return;
                }

                if (connType == "win")
                {
                    sqlConnString = GetSqlServerConnectionString(connServer, connDb);
                }
                else
                {
                    if (connUser == null || connPass == null)
                    {
                        Console.WriteLine("You must provide user and pass if you are not going to use windows auth");
                        return;
                    }
                    sqlConnString = GetSqlServerConnectionString(connServer, connDb, connUser, connPass);
                }

                // change true to argument for Triggers
                // change false to argument for Views
                // change true to argument for GuiDAs String
                SqlServerToSQLite.ConvertSqlServerToSQLiteDatabase(sqlConnString, sqlitePath, null, null,
                null, null, true, false, true, true);

                //string sqlitePath = txtSQLitePath.Text.Trim();
                //    SqlConversionHandler handler = new SqlConversionHandler(delegate (bool done,
                //        bool success, int percent, string msg)
                //    {
                //        Invoke(new MethodInvoker(delegate ()
                //        {
                //            UpdateSensitivity();
                //            lblMessage.Text = msg;
                //            pbrProgress.Value = percent;

                //            if (done)
                //            {
                //                btnStart.Enabled = true;
                //                this.Cursor = Cursors.Default;
                //                UpdateSensitivity();

                //                if (success)
                //                {
                //                    MessageBox.Show(this,
                //                        msg,
                //                        "Conversion Finished",
                //                        MessageBoxButtons.OK,
                //                        MessageBoxIcon.Information);
                //                    pbrProgress.Value = 0;
                //                    lblMessage.Text = string.Empty;
                //                }
                //                else
                //                {
                //                    if (!_shouldExit)
                //                    {
                //                        MessageBox.Show(this,
                //                            msg,
                //                            "Conversion Failed",
                //                            MessageBoxButtons.OK,
                //                            MessageBoxIcon.Error);
                //                        pbrProgress.Value = 0;
                //                        lblMessage.Text = string.Empty;
                //                    }
                //                    else
                //                        Application.Exit();
                //                }
                //            }
                //        }));
                //    });
                //    SqlTableSelectionHandler selectionHandler = new SqlTableSelectionHandler(delegate (List<TableSchema> schema)
                //    {
                //        List<TableSchema> updated = null;
                //        Invoke(new MethodInvoker(delegate
                //        {
                //                // Allow the user to select which tables to include by showing him the 
                //                // table selection dialog.
                //            TableSelectionDialog dlg = new TableSelectionDialog();
                //            DialogResult res = dlg.ShowTables(schema, this);
                //            if (res == DialogResult.OK)
                //                updated = dlg.IncludedTables;
                //        }));
                //        return updated;
                //    });

                //    FailedViewDefinitionHandler viewFailureHandler = new FailedViewDefinitionHandler(delegate (ViewSchema vs)
                //    {
                //        string updated = null;
                //        Invoke(new MethodInvoker(delegate
                //        {
                //            ViewFailureDialog dlg = new ViewFailureDialog();
                //            dlg.View = vs;
                //            DialogResult res = dlg.ShowDialog(this);
                //            if (res == DialogResult.OK)
                //                updated = dlg.ViewSQL;
                //            else
                //                updated = null;
                //        }));

                //        return updated;
                //    });

                //    string password = txtPassword.Text.Trim();
                //    if (!cbxEncrypt.Checked)
                //        password = null;
                //    SqlServerToSQLite.ConvertSqlServerToSQLiteDatabase(sqlConnString, sqlitePath, password, handler,
                //        selectionHandler, viewFailureHandler, cbxTriggers.Checked, createViews, chkBox_treatGuidAsString.Checked);
                //}




            });

            cmd.Invoke(args);



        }
        static string GetSqlServerConnectionString(string address, string db)
        {
            string res = @"Data Source=" + address.Trim() +
                    ";Initial Catalog=" + db.Trim() + ";Integrated Security=SSPI;";
            return res;
        }
        static string GetSqlServerConnectionString(string address, string db, string user, string pass)
        {
            string res = @"Data Source=" + address.Trim() +
                ";Initial Catalog=" + db.Trim() + ";User ID=" + user.Trim() + ";Password=" + pass.Trim();
            return res;
        }

        //private static string GetSqlServerConnectionString(string address, string db)
        //{
        //    string res = @"Data Source=" + address.Trim() +
        //            ";Initial Catalog=" + db.Trim() + ";Integrated Security=SSPI;";
        //    return res;
        //}
        //private static string GetSqlServerConnectionString(string address, string db, string user, string pass)
        //{
        //    string res = @"Data Source=" + address.Trim() +
        //        ";Initial Catalog=" + db.Trim() + ";User ID=" + user.Trim() + ";Password=" + pass.Trim();
        //    return res;
        //}
    }
}