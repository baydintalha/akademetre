<%@ Page Language="C#" AutoEventWireup="true" CodeFile="akademetre.aspx.cs" Inherits="akademetre" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="https://cdn.datatables.net/1.13.1/css/jquery.dataTables.min.css" rel="stylesheet">
    <link href="/dist/assets/css/style.dark.bundle.css" rel="stylesheet" type="text/css" />
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form class="wrapper d-flex flex-column flex-row-fluid" id="kt_wrapper" runat="server">
		<div class="content d-flex flex-column flex-column-fluid" id="kt_content">
			<div class="post d-flex flex-column-fluid" id="kt_post">
				<div id="kt_content_container" class="container-xxl">
					<div class="d-flex flex-column flex-lg-row">
						<div class="flex-column flex-lg-row-auto w-100 w-lg-275px mb-10 mb-lg-0">
							<div class="card card-flush mb-0">
                                <div class="card-header flex-nowrap pt-5">
				                    <h3 class="card-title align-items-start flex-column">
					                    <span class="card-label fw-bolder text-dark">Referanslar</span>
				                    </h3>
			                    </div>
								<div class="card-body">
									<div class="menu menu-column menu-rounded menu-state-bg menu-state-title-primary">
                                        <asp:Repeater runat="server" ID="ReferansRepeater">
                                            <ItemTemplate>
                                                <span class="menu-item mb-3 text-gray-900">
											        <span class="menu-link">
												        <span class="menu-icon">
													        <span class="svg-icon svg-icon-6 svg-icon-warning me-3">
														        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">
															        <path d="M22 12C22 17.5 17.5 22 12 22C6.5 22 2 17.5 2 12C2 6.5 6.5 2 12 2C17.5 2 22 6.5 22 12ZM12 6C8.7 6 6 8.7 6 12C6 15.3 8.7 18 12 18C15.3 18 18 15.3 18 12C18 8.7 15.3 6 12 6Z" fill="currentColor" />
														        </svg>
													        </span>
												        </span>
												        <span class="menu-title fw-bold"><%# Eval("ad") %></span>
											        </span>
										        </span>
                                            </ItemTemplate>
                                        </asp:Repeater>
									</div>
								</div>
							</div>
						</div>
						<div class="flex-lg-row-fluid ms-lg-7 ms-xl-10">
							<div class="card">
								<div class="card-header flex-nowrap pt-5">
				                    <h3 class="card-title align-items-start flex-column">
					                    <span class="card-label fw-bolder text-dark">İşe Alım İstatistikleri</span>
				                    </h3>
                                    <asp:Button ID="vericek" CssClass="btn btn-primary" Text="Veri Çek" runat="server" OnClick="vericek_Click" />
			                    </div>
			                    <div class="card-body pt-5 ps-6">
				                    <div id="kt_charts_widget_5" class="min-h-auto"></div>
			                    </div>
							</div>
						</div>
					</div>
				</div>
			</div>
		</div>
    </form>
    <script>var hostUrl = "/dist/assets/";</script>
	<script src="/dist/assets/plugins/global/plugins.bundle.js"></script>
	<script src="/dist/assets/js/scripts.bundle.js"></script>
	<script src="/dist/assets/plugins/custom/datatables/datatables.bundle.js"></script>
	<script src="/dist/assets/js/widgets.bundle.js"></script>
    <script>
		var KTChartsWidget5={init:function(){!function(){var e=document.getElementById("kt_charts_widget_5");if(e){var a=KTUtil.getCssVariableValue("--bs-border-dashed-color"),t={series:[{name: 'Total Watching',data:[<%=yillik_java%>],show:!1}],chart:{type:"bar",height:350,toolbar:{show:!1}},plotOptions:{bar:{borderRadius:4,horizontal:!0,distributed:!0,barHeight:23}},dataLabels:{enabled:!1},legend:{show:!1},colors:["#3E97FF","#F1416C","#50CD89","#FFC700","#7239EA","#50CDCD","#3F4254"],xaxis:{categories:[<%="\""+departman_java+"\""%>],labels:{formatter:function(e){return e+"W"},style:{colors:KTUtil.getCssVariableValue("--bs-gray-400"),fontSize:"14px",fontWeight:"600",align:"left"}},axisBorder:{show:!1}},yaxis:{labels:{style:{colors:KTUtil.getCssVariableValue("--bs-gray-800"),fontSize:"14px",fontWeight:"600"},offsetY:2,align:"left"}},grid:{borderColor:a,xaxis:{lines:{show:!0}},yaxis:{lines:{show:!1}},strokeDashArray:4}},l=new ApexCharts(e,t);setTimeout((function(){l.render()}),200)}}()}};"undefined"!=typeof module&&(module.exports=KTChartsWidget5),KTUtil.onDOMContentLoaded((function(){KTChartsWidget5.init()}));
	</script>
</body>
</html>
