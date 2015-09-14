var dayNum = 31;
var group_id;
var group_name;
var cwidth = document.documentElement.clientWidth * 0.8;
var now = new Date();
var sdate;
var t;
//var currenttime=now.getFullYear()+'-'+(now.getMonth()+1)+'-'+now.getDay()+' '+now.getHours()+':'+now.getMinutes()+':'+now.getSeconds();
//獲取從當月往前推12個月
Ext.define("gigade.count", {
    extend: 'Ext.data.Model',
    fields: [
    { name: "datetype", type: "string" },
    { name: "totalcount", type: "int" }
    ]
});

var DayStore = Ext.create('Ext.data.Store', {
    model: 'gigade.count',
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        //url: "/Redirect/GetCountdate",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'daydata'
        }
    }
});
var WeekStore = Ext.create('Ext.data.Store', {
    model: 'gigade.count',
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        //url: "/Redirect/GetCountdate",
        actionMethods: 'post',
        reader: {
            type: 'json'
            //root: 'weekdata'
        }
    }
});
var HourStore = Ext.create('Ext.data.Store', {
    model: 'gigade.count',
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        //url: "/Redirect/GetCountdate",
        actionMethods: 'post',
        reader: {
            type: 'json'
            //root: 'hourdata'
        }
    }
});

Ext.onReady(function ()
{
    group_id = document.getElementById("group_id").value == null ? 0 : document.getElementById("group_id").value;
    group_name = document.getElementById("group_name").value;

    var titleForm = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        renderTo: Ext.getBody(),
        id: 'titleForm',
        border: false,
        //width: 200,
        plain: true,
        bodyPadding: 20,
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                html: '<div class="contain-title" >' + group_name + '群組報表</div>',
                frame: false,
                margin: '0 0 3 0',
                border: false
            },
            {
                xtype: 'fieldcontainer',
                items: [
                {
                    xtype: "datefield",
                    fieldLabel: '選擇報表日期',
                    id: 'select_month',
                    name: 'select_month',
                    format: 'Y/m',
                    allowBlank: false,
                    submitValue: true,
                    value:new Date(),
                    listeners:
                        {
                            select: function ()
                            {
                                sdate = Ext.getCmp('select_month').getRawValue().toString();
                                t = sdate.substr(0, 4) + '年' + sdate.substr(5, 2) + '月';
                                Ext.getCmp('gdDay').setTitle(t + '  每日記錄');
                                Ext.getCmp('gdWeek').setTitle(t + '  每周記錄');
                                Ext.getCmp('gdHour').setTitle(t + '  每小時記錄');
                                Ext.getCmp('theader').setText(GetDayHeader());
                                LoadStore();
                            }
                        }
                },
                    {//建立時間
                        xtype: 'displayfield',
                        fieldLabel: "報表輸出時間",
                        value: Ext.Date.format(now, 'Y-m-d H:i:s')
                    }
                ]
            }
        ]
    });
    if (Ext.getCmp('select_month').getRawValue() != "")
    {
        //sdate = new Date(Ext.getCmp('select_month').getRawValue());
        sdate = Ext.getCmp('select_month').getRawValue().toString();
        //t = sdate.getFullYear() + '年' + (sdate.getMonth() + 1) + '月';
        t = sdate.substr(0, 4) + '年' + sdate.substr(5, 2) + '月';
    }
    var gdDay = Ext.create('Ext.grid.Panel', {
        id: 'gdDay',
        title: t + ' 每日記錄',
        store: DayStore,
        columnLines: false,
        margin: '0 0 20 20',
        border: false,
        frame: false,
        features: [{
            ftype: 'summary'
        }],
        columns: [{
            text: GetDayHeader(),
            id:'theader',
            columns: [
                {
                    header: '日期', dataIndex: 'datetype', width: cwidth * 0.6, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store)
                    {
                        if (value.toString().indexOf('*') > 0)
                        {
                            return value.toString().substring(0, value.length - 1);
                        }
                        else
                        {
                            return value;
                        }
                    },
                    summaryType: 'count',
                    summaryRenderer: function (value, summaryData, dataIndex)
                    {
                        return '<div style="color:red;">總計</div';
                    }
                },
                {
                    header: '<div style="background-color:#66DDEE;">點閱次數</div>', dataIndex: 'totalcount', width: cwidth * 0.4, align: 'center', summaryType: 'sum',
                    summaryRenderer: function (value, summaryData, dataIndex)
                    {
                        return '<div style="color:red;">' + value + '</div';
                    }
                }
            ]
        }  //{ header: '週末', dataIndex: 'holiday', width: 100, align: 'center' }
        ],
        viewConfig: {
            forceFit: true, getRowClass: function (record, rowIndex, rowParams, store)
            {
                //alert(record.data.type);
                if (record.data.datetype.toString().indexOf('*') > 0)
                {
                    // alert(record.data.type);
                    return 'isholiday';
                }
            }
        }
    });
    var gdWeek = Ext.create('Ext.grid.Panel', {
        id: 'gdWeek',
        title: t + ' 每周記錄',
        store: WeekStore,
        columnLines: false,
        margin: '0 0 20 400',
        //align:'center',
        border: false,
        frame: false,
        features: [{
            ftype: 'summary'
        }],
        columns: [
            {
                text: '<p style="font-size:15px;letter-spacing:4em; ">日一二三四五六</p>',
                columns: [
            {
                header: '星期', dataIndex: 'datetype', width: cwidth * 0.2, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store)
                {
                    switch (value)
                    {
                        case '0':
                            return '日';
                            break;
                        case '1':
                            return '一';
                            break;
                        case '2':
                            return '二';
                            break;
                        case '3':
                            return '三';
                            break;
                        case '4':
                            return '四';
                            break;
                        case '5':
                            return '五';
                            break;
                        case '6':
                            return '六';
                            break;
                    }
                }
                , summaryType: 'count',
                summaryRenderer: function (value, summaryData, dataIndex)
                {
                    return '<div style="color:red;">總計</div';
                }
            },
            {
                header: '<div style="background-color:#66DDEE;">點閱次數</div>', dataIndex: 'totalcount', width: cwidth * 0.2, align: 'center', summaryType: 'sum',
                summaryRenderer: function (value, summaryData, dataIndex)
                {
                    return '<div style="color:red;">' + value + '</div';
                }
            }
                ]
            }
        ]
    });
    var gdHour = Ext.create('Ext.grid.Panel', {
        id: 'gdHour',
        title: t + ' 每小時記錄',
        store: HourStore,
        columnLines: false,
        margin: '0 0 20 20',
        border: false,
        frame: false,
        features: [{
            ftype: 'summary'
        }],
        columns: [
            {
                text: GetHourHeader(),              
                columns: [
             {
                header: '小時', dataIndex: 'datetype', width: cwidth * 0.6, align: 'center', summaryType: 'count',
                summaryRenderer: function (value, summaryData, dataIndex)
                {
                    return '<div style="color:red;">總計</div';
                }
            },
            {
                header: '<div style="background-color:#66DDEE;">點閱次數</div>', dataIndex: 'totalcount', width: cwidth * 0.4, align: 'center', summaryType: 'sum',
                summaryRenderer: function (value, summaryData, dataIndex)
                {
                    return '<div style="color:red;">' + value + '</div';
                }
            }
                ]
            }
        ]
    });


    Ext.create('Ext.container.Viewport', {
        layout: 'anchor',//vbox
        items: [titleForm, gdDay, gdWeek, gdHour],
        renderTo: Ext.getBody(),
        //margin:'10 5 0 0',
        autoScroll: true,
        // width:document.documentElement.clientWidth*0.6,
        listeners: {
            resize: function ()
            {
                gdDay.width = cwidth;
                gdWeek.width = cwidth * 0.4;
                gdHour.width = cwidth;
                this.doLayout();
            },
            render: function ()
            {
                LoadStore();
            }
        }
    });
    //ToolAuthority();
})
//加載倆個store的數據
function LoadStore()
{
    Ext.Ajax.request({
        url: '/Redirect/GetRedirectClickCount',
        method: 'post',
        params: {
            selectDate: Ext.getCmp('select_month').getRawValue(),
            redirect_id: document.getElementById('redirect_id').value,
            group_id: group_id
        },
        success: function (form, action)
        {
            var responseJson = Ext.JSON.decode(form.responseText);
            var daydata = responseJson.daydata;
            var weekdata = responseJson.weekdata;
            var hourdata = responseJson.hourdata;
            DayStore.loadData(daydata);
            WeekStore.loadData(weekdata);
            HourStore.loadData(hourdata);
        },
        failure: function ()
        {
            //Ext.Msg.alert("提示","您輸入的料位條碼不符,請查找對應的料位!");
        }
    });

}
function GetDayHeader()
{
    var header = "";
    var d = new Date(Ext.getCmp('select_month').getRawValue());
    var cyear = d.getFullYear();
    var cmonth = d.getMonth() + 1;
    var cdate = new Date(cyear, cmonth, 0);
    var cdays = cdate.getDate(cdate);
    for (var i = 1; i <= cdays; i++)
    {
        header += '<span style="font-size:15px;margin-left:28px; ">' + i.toString() + '</span>';
    }
    //return '<p style="font-size:15px;letter-spacing:1em; ">' + header + '</p>';
    return header;
}
function GetHourHeader()
{
    var header = "";
    for (var i = 0; i <= 23; i++)
    {
        header += '<span style="font-size:15px;margin-left:40px;">' + i.toString() + '</span>';
    }
    //return '<p style="font-size:15px;letter-spacing:1em; ">'+ header+'</p>';
    return header;
}