$(document).ready(function () {

    //查询所有报告
    $.getJSON("/System/getAllReports", {}, function (data) {        
        for (var i = 0; i < data.length; i++) {
            data[i].Number = i + 1;
            data[i].Operation = getOperation(data[i].ReportID, data[i].State);
        }

        var testdata3 = { 'code': '000', 'data': data };

        $('#tableReport').yhhDataTable({
            'paginate': {
                'changeDisplayLen': true,
                'type': 'updown',
                'visibleGo': true
            },
            'tbodyRow': {
                'zebra': true,
                'write': function (d) {
                    return '<tr><td>' + d.Number + '</td><td>' + d.DocNumber + '</td><td>' + d.DocName + '</td><td>' + d.State + '</td><td>' + d.Operation + '</td></tr>';
                }
            },
            'tbodyData': {
                'enabled': true,  /*是否传入表格数据*/
                'source': testdata3 /*传入的表格数据*/
            },
            'backDataHandle': function (d) {
                if (d.code == '000') {
                    return d.data;
                } else {
                    alert('出错信息');
                    return [];
                }
            }
        });
    });

});

//获取四个操作按钮的html代码
function getOperation(ReportID, State) {
    var strHtml = "";

    if (State.trim() == "作废") {
        strHtml = "<div class='btn-group'>"
                + "<button class='btn btn-xs btn-warning' onclick='rebornAReport(" + ReportID + ")'><i class='ace-icon fa fa-retweet bigger-120'></i> 恢复 </button>"
                + "</div>";
    }
    else {
        strHtml = "<div class='btn-group'>"
                + "<button class='btn btn-xs btn-success'><i class='ace-icon fa fa-search-plus bigger-120'></i> 查阅 </button>"
                + "<button class='btn btn-xs btn-info'><i class='ace-icon fa fa-pencil bigger-120'></i> 编辑 </button>"
                + "<button class='btn btn-xs btn-danger' onclick='destroyAReport(" + ReportID + ")'><i class='ace-icon fa fa-trash-o bigger-120'></i> 删除 </button>"
                + "<button class='btn btn-xs btn-success'><i class='ace-icon fa fa-download bigger-120'></i> 下载 </button>"
                + "</div>";
    }

    return strHtml;
}

//删除一条报告
function destroyAReport(ReportID) {
    var confirmed = confirm("您确定要删除该报告吗？");
    if (confirmed) {
        $.getJSON("/System/destroyAReport", { ReportID: ReportID }, function (data) {
            
        });
    }
}



