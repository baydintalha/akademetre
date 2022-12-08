<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link href="https://cdn.datatables.net/1.13.1/css/jquery.dataTables.min.css" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.2.3/dist/css/bootstrap.min.css" rel="stylesheet">
    <style>
        .dataTables_wrapper {
            background: #212529!important;
            color: white!important;
        }
        .dataTables_wrapper .dataTables_length select {
            color: white!important;
        }
        .form-control {
            color: white!important;
            background-color:transparent!important;
            border-color:#767676!important;
        }
        .form-control:hover {
            color: white!important;
            background-color:transparent!important;
        }
        .form-control:focus {
            color: white!important;
            background-color:transparent!important;
        }
        .dataTables_wrapper .dataTables_filter input {
            color: white!important;
        }
    </style>
</head>
<body style="height: 100%;background-color: #151521;">
    <form id="form2" runat="server" class="container" style="height: 100vh;">
        <div class="container">
	        <div class="d-flex flex-column flex-row">
		        <div class="flex-column">
			        <div class="card card-flush my-3 text-white" style="margin-top:10vh!important; background-color:#1e1e2d;">
                        <asp:Panel runat="server" ID="formGirdileri" CssClass="card-body">
                            <div class="row">
                                <div class="col-6">
                                    <label for="inputEmail4" class="form-label">İSİM</label>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="isim" />
                                </div>
                                <div class="col-6">
                                    <label for="inputPassword4" class="form-label">SOYİSİM</label>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="soyisim" />
                                </div>
                                <div class="col-12">
                                    <label for="inputAddress" class="form-label">ADRES</label>
                                    <asp:TextBox runat="server" CssClass="form-control" ID="adres" />
                                </div>
                                <div class="col-12">
                                    <label for="inputAddress2" class="form-label">MAİL</label>
                                    <asp:TextBox runat="server" TextMode="Email" CssClass="form-control" ID="mail"/>
                                </div>
                                <div class="col-12 d-flex flex-row-reverse mt-3">
                                    <asp:Button ID="EXCEL" Text="EXCEL" runat="server" CssClass="btn btn-primary" OnClick="EXCEL_Click" />
                                    <asp:Button ID="temizle" Text="TEMİZLE" runat="server" CssClass="btn btn-primary me-2" OnClick="temizle_Click" />
                                    <asp:Button ID="kayit" Text="EKLE" runat="server" CssClass="btn btn-primary me-2" OnClick="kayit_Click" />
                                </div>
                            </div>
                        </asp:Panel>
			        </div>
		        </div>
		        <div class="flex-row-fluid pt-10">
			        <div class="card" style="background-color:#1e1e2d;padding: 6px;">
				        <div class="card-body p-0">
						    <div class="d-block">
                                <asp:Panel runat="server" ID="tablo" CssClass="card-body p-0">
                                    <div class="d-block">
                                        <table id="veriTablosu" class="table table-striped table-dark table-hover" style="width:100%">
                                            <thead class="table-dark">
                                                <tr>
                                                    <th>İSİM</th>
                                                    <th>SOYİSİM</th>
                                                    <th>ADRES</th>
                                                    <th>MAİL</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <asp:Repeater runat="server" ID="veriRepeater">
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td><%# Eval("isim") %></td>
                                                            <td><%# Eval("soyisim") %></td>
                                                            <td><%# Eval("adres") %></td>
                                                            <td><%# Eval("mail") %></td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tbody>
                                        </table>
                                    </div>
                                </asp:Panel>
						    </div>
				        </div>
			        </div>
		        </div>
	        </div>
        </div>
    </form>
    <script src="https://code.jquery.com/jquery-3.5.1.js"></script>
    <script src="https://cdn.datatables.net/1.13.1/js/jquery.dataTables.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.2.3/dist/js/bootstrap.bundle.min.js"></script>
    <script>
        $(document).ready(function () {
            $('#veriTablosu').DataTable();
        });
    </script>
</body>
</html>
