var pageSize = 25;
/**********************************************************************群組管理主頁面**************************************************************************************/
var searchStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
          { 'txt': '全部狀態', 'value': '-1' },
          { 'txt': '新建', 'value': '0' },
          { 'txt': '顯示', 'value': '1' },
          { 'txt': '隱藏', 'value': '2' },
          { 'txt': '下檔', 'value': '3' }
    ]

});
var dateStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
            { 'txt': '日期區間', 'value': '0' },
            { 'txt': '上線時間', 'value': '1' },
            { 'txt': '下線時間', 'value': '2' }
    ]

});
//群組管理Model
Ext.define('gigade.NewsContent', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "news_id", type: "int" },
        { name: "user_id", type: "int" },
        { name: "news_title", type: "string" },
        { name: "news_content", type: "string" },
        { name: "news_sort", type: "int" },
        { name: "news_status", type: "int" },
        { name: "news_show_start", type: "int" },
        { name: "news_show_end", type: "int" },
        { name: "news_createdate", type: "int" },
        { name: "news_updatedate", type: "int" },
        { name: 'news_ipfrom', type: 'string' },
        { name: 'user_username', type: 'string' },
        { name: "s_news_show_start", type: "string" },
        { name: "s_news_show_end", type: "string" },
        { name: "s_news_createdate", type: "string" },
        { name: "s_news_updatedate", type: "string" }
    ]
});



var NewsContentStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.NewsContent',
    proxy: {
        type: 'ajax',
        url: '/News/GetNewsList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("NewsContentGrid").down('#edit').setDisabled(selections.length == 0);
            var row = Ext.getCmp("NewsContentGrid").getSelectionModel().getSelection();
            if (row != "") {
                if (row[0].data.news_status == 3) {
                    //alert(row[0].data.news_status + "+" + row[0].data.news_id);
                    Ext.getCmp("NewsContentGrid").down('#edit').setDisabled(true);
                }
            }

        }
    }
});

NewsContentStore.on('beforeload', function () {
    Ext.apply(NewsContentStore.proxy.extraParams,
        {
            searchCon: Ext.getCmp('searchCon').getValue(),
            search_con: Ext.getCmp('search_con').getValue(),
            date: Ext.getCmp('date').getValue(),
            start_time: Ext.getCmp('start_time').getValue(),
            end_time: Ext.getCmp('end_time').getValue()
        });
});

var EditTpl = new Ext.XTemplate(
        '<a href=javascript:TranToDetial("/News/LogList","{news_id}")>' + "記錄" + '</a> '
    );

Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 100,
        border: 0,
        bodyPadding: 10,
        width: document.documentElement.clientWidth,
        items: [
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'combobox',
                        fieldLabel: '查詢條件',
                        labelWidth: 70,
                        id: 'searchCon',
                        name: 'searchCon',
                        store: searchStore,
                        editable:false,
                        displayField: 'txt',
                        valueField: 'value',
                        value: '',
                        margin:'0 5 0 0',
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '標題',
                        labelWidth: 40,
                        id: 'search_con',
                        name: 'search_con'
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                     {
                         xtype: 'combobox',
                         fieldLabel: '日期條件',
                         labelWidth: 70,
                         id: 'date',
                         name: 'date',
                         store: dateStore,
                         editable: false,
                         displayField: 'txt',
                         valueField: 'value',
                         value: '',
                         margin: '0 5 0 0',
                     },
                      {                          
                         xtype: "datetimefield",
                          id: 'start_time',
                          name: 'start_time',
                          format: 'Y-m-d H:i:s',
                          allowBlank: true,
                          editable: false,
                          time: { hour: 00, min: 00, sec: 00 },//標記結束時間00:00:00
                          
                          listeners: {
                              select: function (a, b, c) {
                                  var start = Ext.getCmp("start_time");
                                  var end = Ext.getCmp("end_time");
                                  if (end.getValue() == null) {
                                      end.setValue(setNextMonth(start.getValue(), 1));
                                  }
                                  else if (end.getValue() < start.getValue()) {
                                       
                                      end.setValue(setNextMonth(start.getValue(), 1));
                                  }
                              }
                          }
                      },
                      {
                          xtype: 'displayfield',
                          margin: '0 2 0 2',
                          value: "~"
                      },
                      {
                          xtype: "datetimefield",
                          id: 'end_time',
                          name: 'end_time',
                          editable: false,
                          allowBlank: true,
                          format: 'Y-m-d H:i:s',
                          time: { hour: 23, min: 59, sec: 59 },//標記結束時間00:00:00
                          value: '',
                          listeners: {
                              select: function (a, b, c) {
                                  var start = Ext.getCmp("start_time");
                                  var end = Ext.getCmp("end_time");
                                  if (start.getValue() == null)
                                  {
                                      start.setValue(setNextMonth(end.getValue(), -1));
                                  }
                                  else if ( end.getValue() < start.getValue()) {
                                       
                                      start.setValue(setNextMonth(end.getValue(),-1));
                                  }
                              }
                          }
                      },
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'button',
                        text: '查詢',
                        iconCls: 'icon-search',
                        handler: Query,
                        margin:'0 5 0 0 '

                    },
                    {
                        xtype: 'button',
                        text: '重置',
                        handler: function () {
                            Ext.getCmp('searchCon').setValue('');
                            Ext.getCmp('search_con').setValue('');
                            Ext.getCmp('date').setValue('');
                            Ext.getCmp('start_time').setValue(null);
                            Ext.getCmp('end_time').setValue(null);
                        }
                    }              
                ]

            }
        ]
    });

    var NewsContentGrid = Ext.create('Ext.grid.Panel', {
        id: 'NewsContentGrid',
        store: NewsContentStore,
        //width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            {
                header: "編號", dataIndex: 'news_id', width: 60, align: 'center'
            },
                          { header: '記錄', width: 60, align: 'center', xtype: 'templatecolumn', tpl: EditTpl },
            {
                header: "上稿者", dataIndex: 'user_username', width: 150, align: 'center'
            },
            {
                header: "標題", dataIndex: 'news_title', width: 200, align: 'center'
            },
            { header: "排序", dataIndex: 'news_sort', width: 150, align: 'center' },
            {
                header: "狀態", dataIndex: 'news_status', width: 150, align: 'center',
                renderer: function (value) {
                    if (value == 1) {
                        return "顯示";
                    }
                    else if (value == 0) {
                        return "<span style= 'color:red'>新建</span>";
                    }
                    else if (value == 2) {
                        return "<span style= 'color:red'>隱藏</span>";
                    }
                    else if (value == 3) {
                        return "<span style= 'color:red'>下檔</span>";
                    }
                }
            },
         {
             header: "上線時間", dataIndex: 's_news_show_start', width: 150, align: 'center',
             renderer: function (value) {
                 // alert(value + "    " + Today2());
                 if (value > Today2()) {
                     return "<span style='color:red'>" + value + "</span>";
                 }
                 else {
                     return value;
                 }
             }
         },
         {
             header: "下線時間", dataIndex: 's_news_show_end', width: 150, align: 'center',
             renderer: function (value) {
                 //alert(value + "    " + Today());
                 if (value < Today2()) {
                     return "<span style='color:red'>" + value + "</span>";
                 }
                 else {
                     return value;
                 }
             }
         }

        ],
        tbar: [
           { xtype: 'button', text: ADD, id: 'add', iconCls: 'icon-user-add', handler: onAddClick },
           { xtype: 'button', text: EDIT, id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: NewsContentStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }),
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }

        },
        selModel: sm
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [frm,NewsContentGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                NewsContentGrid.width = document.documentElement.clientWidth;
                NewsContentGrid.height = document.documentElement.clientHeight-100;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
   // NewsContentStore.load({ params: { start: 0, limit: 25 } });
});

/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {

    //addWin.show();
    editFunction(null, NewsContentStore);

}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("NewsContentGrid").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], NewsContentStore);
    }
}

function Query() {
    NewsContentStore.removeAll();
    var searchCon=Ext.getCmp('searchCon').getValue();
    var search_con = Ext.getCmp('search_con').getValue();
    var date = Ext.getCmp('date').getValue();
    var start_time = Ext.getCmp('start_time').getValue();
    var end_time = Ext.getCmp('end_time').getValue();
    if (date != "" && (start_time == null || end_time == null)) {
        Ext.Msg.alert(INFORMATION, '請選擇查詢條件');
    } else {
        if (searchCon == "" && search_con == "" && (date == "" || (start_time == null || end_time == null))) {
            Ext.Msg.alert(INFORMATION, '請選擇查詢條件');
        }
        else {
            Ext.getCmp('NewsContentGrid').store.loadPage(1, {
                params: {
                    searchCon: Ext.getCmp('searchCon').getValue(),
                    search_con: Ext.getCmp('search_con').getValue(),
                    date: Ext.getCmp('date').getValue(),
                    start_time: Ext.getCmp('start_time').getValue(),
                    end_time: Ext.getCmp('end_time').getValue()

                }

            });
        }
    }
}

function TranToDetial(url, news_id) {

    var urlTran = url + '?news_id=' + news_id;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#detial');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'detial',
        title: '最新消息歷史記錄',
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();

}
function Today() {
    var d;
    d = new Date();                             // 创建 Date 对象。
    d.setDate(d.getDate());
    return d;
}
//function Today() {
//    var d;
//    var s = "";
//    d = new Date();                             // 创建 Date 对象。
//    var year = d.getFullYear();
//    var month = d.getMonth() + 1;
//    if (month < 10) {
//        month = "0" + month;
//    }
//    var day = d.getDate();
//    if (day < 10) {
//        day = "0" + day;
//    }
//    var hour = d.getHours();
//    if (hour < 10) {
//        hour = "0" + hour;
//    }
//    var minutes = d.getMinutes();
//    if (minutes < 10) {
//        minutes = "0" + minutes;
//    }
//    var sec = d.getSeconds();
//    if (sec < 10) {
//        sec = "0" + sec;
//    }
//    s = year + "-" + month + "-" + day + " " + hour + ":" + minutes + ":" + sec;
//    return s;                                 // 返回日期。
//}
function Today2() {
    var d;
    d = new Date();                             // 创建 Date 对象。
    d.setDate(d.getDate());
    return d;
}
//function Today2() {
//    var d;
//    var s = "";
//    d = new Date();                             // 创建 Date 对象。
//    var year = d.getFullYear();
//    var month = d.getMonth() + 1;
//    if (month < 10) {
//        month = "0" + month;
//    }
//    var day = d.getDate();
//    if (day < 10) {
//        day = "0" + day;
//    }
//    var hour = d.getHours();
//    if (hour < 10) {
//        hour = "0" + hour;
//    }
//    var minutes = d.getMinutes();
//    if (minutes < 10) {
//        minutes = "0" + minutes;
//    }
//    var sec = d.getSeconds();
//    if (sec < 10) {
//        sec = "0" + sec;
//    }
//    s = year + "-" + month + "-" + day + " " + hour + ":" + minutes + ":" + sec;
//    return s;                                 // 返回日期。
//}

function Tomorrow() {
    var d;
    d = new Date();                             // 创建 Date 对象。
    d.setDate(d.getDate() + 1);
    return d;
}

setNextMonth = function (source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0) {
        s.setHours(0, 0, 0);
    }
    else if (n > 0) {
        s.setHours(23, 59, 59);
    }
    return s;
}




