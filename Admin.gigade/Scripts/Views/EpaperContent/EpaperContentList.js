var pageSize = 25;

//查詢條件
var searchStore = Ext.create('Ext.data.Store', {
    fields: ['searchCondition', 'searchValue'],
    data: [
    { "searchCondition": "所有資料", "searchValue": "0" },
    { "searchCondition": "標題", "searchValue": "1" },
    { "searchCondition": "短標題", "searchValue": "2" },
    { "searchCondition": "上稿者", "searchValue": "3" }
    ]
});
var sizeStore = Ext.create('Ext.data.Store', {
    fields: ['sizeText', 'sizeValue'],
    data: [
    { "sizeText": "所有尺寸", "sizeValue": "0" },
    { "sizeText": "725px", "sizeValue": "725px" },
    { "sizeText": "900px", "sizeValue": "900px" }
    ]
});
var activeStatusStore = Ext.create('Ext.data.Store', {
    fields: ['activeStatusText', 'activeStatusValue'],
    data: [
    { "activeStatusText": "所有狀態", "activeStatusValue": "-1" },
    { "activeStatusText": "新建", "activeStatusValue": "0" },
    { "activeStatusText": "顯示", "activeStatusValue": "1" },
    { "activeStatusText": "隱藏", "activeStatusValue": "2" },
    { "activeStatusText": "下檔", "activeStatusValue": "3" }
    ]
});
//日期條件
var dateStore = Ext.create('Ext.data.Store', {
    fields: ['searchCondition', 'searchValue'],
    data: [
    { "searchCondition": "所有日期", "searchValue": "0" },
    { "searchCondition": "上線日期", "searchValue": "1" },
    { "searchCondition": "下線日期", "searchValue": "2" }
    ]
});

//model
Ext.define('gigade.EpaperContentModel', {
    extend: 'Ext.data.Model',
    fields: [
    { name: "epaper_id", type: "int" },
    { name: "user_id", type: "int" },
    { name: "user_username", type: "string" },
    { name: "epaper_title", type: "string" },
    { name: "epaper_short_title", type: "string" },
    { name: "epaper_content", type: "string" },
    { name: "epaper_sort", type: "int" },
    { name: "epaper_status", type: "int" },
    { name: "epaper_size", type: "string" },
    { name: "epaper_show_start", type: "string" },
    { name: "epaper_show_end", type: "string" },
    { name: "fb_description", type: "string" },
    { name: "epaper_createdate", type: "string" },
    { name: "epaper_updatedate", type: "string" },
    { name: "epaper_ipfrom", type: "string" },
    { name: "epaperShowStart", type: "string" },
    { name: "epaperShowEnd", type: "string" },
    { name: "epaperCreateDate", type: "string" },
    { name: "epaperUpdateDate", type: "string" },
    { name: "type", type: "int" }

    ]
});

var EpaperContentStore = Ext.create('Ext.data.Store', {
    autoDestroy: true, //自動消除
    pageSize: pageSize,
    model: 'gigade.EpaperContentModel',
    proxy: {
        type: 'ajax',
        url: '/EpaperContent/GetEpaperContentList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});


//前面選擇框 選擇之後顯示編輯刪除
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdEpaper").down('#edit').setDisabled(selections.length == 0);
            var row = Ext.getCmp("gdEpaper").getSelectionModel().getSelection();
            if (row != "") {
                if (row[0].data.epaper_status == 3) {
                    Ext.getCmp("gdEpaper").down('#edit').setDisabled(true);
                }
            }
        }
    }
});
EpaperContentStore.on('beforeload', function () {
    Ext.apply(EpaperContentStore.proxy.extraParams,
    {
        searchCon: Ext.getCmp('searchCon').getValue(),
        search_text: Ext.getCmp('search_text').getValue(),
        dateCon: Ext.getCmp('dateCon').getValue(),
        date_start: Ext.getCmp('timestart').getValue(),
        date_end: Ext.getCmp('timeend').getValue(),
        activeStatus: Ext.getCmp('activeStatus').getValue(),
        sizeCon: Ext.getCmp('sizeCon').getRawValue()

    });
});
var EditTpl = new Ext.XTemplate(
'<a href=javascript:TranToDetial("/EpaperContent/EpaperLogList","{epaper_id}")>' + "記錄" + '</a> '
);

Ext.onReady(function () {
    var searFrm = Ext.create('Ext.form.Panel', {
        id: 'searFrm',
        border: 0,
        layout: 'anchor',
        height: 140,
        width: document.documentElement.clientWidth,
        items: [
        {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            items: [
            {
                xtype: 'combobox',
                store: searchStore,
                id: 'searchCon',
                fieldLabel: '查詢條件',
                displayField: 'searchCondition',
                valueField: 'searchValue',
                width: 180,
                labelWidth: 60,
                margin: '5 5 2 2',
                forceSelection: false,
                editable: false,
                value: '0'
            },
            {
                xtype: 'textfield',
                fieldLabel: "查詢內容",
                width: 180,
                labelWidth: 60,
                margin: '5 0 2 2',
                id: 'search_text',
                name: 'search_text',
                value: ""
            }
            ]
        },
        {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            items: [
            {
                xtype: 'combobox',
                id: 'dateCon',
                name: 'dateCon',
                store: dateStore,
                displayField: 'searchCondition',
                valueField: 'searchValue',
                fieldLabel: '日期條件',
                value: '0',
                editable: false,
                labelWidth: 60,
                width: 180,
                margin: '0 5 0 2'
            },
            {
                xtype: 'datetimefield', allowBlank: true, id: 'timestart', format: 'Y-m-d H:i:s', name: 'serchcontent', editable: false, labelWidth: 60, time: { hour: 00, min: 00, sec: 00 }, listeners: {
                    select: function (a, b, c) {
                        var start = Ext.getCmp("timestart");
                        var end = Ext.getCmp("timeend");
                        if (end.getValue() == null) {
                            end.setValue(setNextMonth(start.getValue(), 1));
                        } else if (end.getValue() < start.getValue()) {
                            end.setValue(setNextMonth(start.getValue(), 1));
                        }
                    }
                }
            },
            {
                xtype: 'displayfield',
                value: '~'
            },
            {
                xtype: 'datetimefield', allowBlank: true, id: 'timeend', format: 'Y-m-d H:i:s', editable: false, name: 'serchcontent', time: { hour: 23, min: 59, sec: 59 }, listeners: { 
                    select: function (a, b, c) {
                        var start = Ext.getCmp("timestart");
                        var end = Ext.getCmp("timeend");
                        if ( start.getValue()== null) {
                            start.setValue(setNextMonth(end.getValue(), -1));
                        }
                        else if (end.getValue() < start.getValue()) {
                            start.setValue(setNextMonth(end.getValue(), -1));
                        }
                    }
                }

            }
            ]
        }
        ,
        {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            items: [
            {
                xtype: 'combobox',
                store: activeStatusStore,
                id: 'activeStatus',
                fieldLabel: '活動狀態',
                displayField: 'activeStatusText',
                valueField: 'activeStatusValue',
                width: 180,
                labelWidth: 60,
                margin: '0 5 5 2',
                forceSelection: false,
                editable: false,
                value: '-1'
            },
            {
                xtype: 'combobox',
                store: sizeStore,
                id: 'sizeCon',
                fieldLabel: '尺寸',
                displayField: 'sizeText',
                valueField: 'sizeValue',
                width: 180,
                labelWidth: 60,
                margin: '0 5 5 2',
                forceSelection: false,
                editable: false,
                value: '0'
            }
            ]
        }
        ],
        buttonAlign: 'left',
        buttons: [
        {
            text: SEARCH,
            iconCls: 'icon-search',
            id: 'btnQuery',
            margin: '5 5 2 5',
            handler: Query
        },
        {
            text: RESET,
            id: 'btn_reset',
            margin: '5 0 2 0',
            listeners: {
                click: function () {
                    Ext.getCmp('searchCon').setValue('0');
                    Ext.getCmp('search_text').setValue("");
                    Ext.getCmp('dateCon').setValue('0');
                    Ext.getCmp('activeStatus').setValue('-1');
                    Ext.getCmp('sizeCon').setValue('0');
                    Ext.getCmp('timestart').setValue(null);
                    Ext.getCmp('timeend').setValue(null);
                }
            }
        },
        ],
    });

    var gdEpaper = Ext.create('Ext.grid.Panel', {
        id: 'gdEpaper',
        store: EpaperContentStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        viewConfig: {
            enableTextSelection: true,
            stripeRows: false,
            getRowClass: function (record, rowIndex, rowParams, store) {
                return "x-selectable";
            }
        },
        columns: [
        { header: "編號", dataIndex: 'epaper_id', width: 60, align: 'center' },
        { header: '記錄', width: 60, align: 'center', xtype: 'templatecolumn', tpl: EditTpl },
        { header: '上稿者', dataIndex: 'user_username', width: 100, align: 'center' },
        { header: '標題', dataIndex: 'epaper_title', width: 300, align: 'center' },
        { header: "短標題", dataIndex: 'epaper_short_title', width: 100, align: 'center' },
        { header: "排序", dataIndex: 'epaper_sort', width: 80, align: 'center' },
        {//0:新建1顯示2隱藏3下檔
            header: '狀態', dataIndex: 'epaper_status', width: 150, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                if (record.data.epaper_status == 1) {
                    return "顯示";
                }
                if (record.data.epaper_status == 0) {
                    return "<span style=' color:red'>新建</span>";
                }
                if (record.data.epaper_status == 2) {
                    return "<span style=' color:red'>隱藏</span>";
                }
                if (record.data.epaper_status == 3) {
                    return "<span style=' color:red'>下檔</span>";
                }
            }
        },
        { header: "尺寸", dataIndex: 'epaper_size', width: 150, align: 'center' },
        {
            header: '上線時間', dataIndex: 'epaperShowStart', width: 150, align: 'center',
            renderer: function (value) {
                if (value > Today2()) {
                    return "<span style='color:red'>" + value + "</span>";
                }
                else {
                    return value;
                }
            }
        },
        {
            header: '下線時間', dataIndex: 'epaperShowEnd', width: 150, align: 'center',
            renderer: function (value) {
                if (value < Today2()) {
                    return "<span style='color:red'>" + value + "</span>";
                }
                else {
                    return value;
                }
            }
        }
        //{ header: BANNERLINKURL, dataIndex: 'fb_description', width: 100, align: 'center' },
        //{ header: BANNERLINKMODE, dataIndex: 'epaper_createdate', width: 80, align: 'center', hidden: true },
        //{ header: BANNERLINKMODE, dataIndex: 'epaper_updatedate', width: 80, align: 'center' },
        //{ header: BANNERSORT, dataIndex: 'epaper_ipfrom', width: 50, align: 'center' },
        //{ header: BANNERSTATUS, dataIndex: 'type', width: 100, align: 'center', hidden: true }

        ],
        tbar: [
        { xtype: 'button', text: ADD, id: 'add', iconCls: 'icon-user-add', handler: onAddClick, hidden: true },
        { xtype: 'button', text: EDIT, id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick }

        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: EpaperContentStore,
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
        layout: 'anchor',
        items: [searFrm, gdEpaper],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdEpaper.width = document.documentElement.clientWidth;
                gdEpaper.height = document.documentElement.clientHeight - 140;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    if (window.parent.Ext.getCmp('ContentPanel').activeTab.name == 'EpaperContentEdit') {
        EpaperContentStore.load({ params: { start: 0, limit: 25 } });
    }
});

//添加
onAddClick = function () {
    var urlTran = '/EpaperContent/EpaperContentAdd';
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#detial');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'detial',
        title: '新增活動列表頁',
        html: window.top.rtnFrame(urlTran),
        name: panel.activeTab.title,
        closable: true
    });
    panel.activeTab.close();
    panel.setActiveTab(copy);
    panel.doLayout();
}
//修改
onEditClick = function () {
    var row = Ext.getCmp("gdEpaper").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else {
        var urlTran = '/EpaperContent/EpaperContentAdd?epaper_id=' + row[0].data.epaper_id;
        var panel = window.parent.parent.Ext.getCmp('ContentPanel');
        var copy = panel.down('#detial');
        if (copy) {
            copy.close();
        }
        copy = panel.add({
            id: 'detial',
            title: '編輯活動列表頁',
            html: window.top.rtnFrame(urlTran),
            name: panel.activeTab.title,
            closable: true
        });
        panel.activeTab.close();
        panel.setActiveTab(copy);
        panel.doLayout();
    }
}
function Query(x) {
    var searchCon = Ext.getCmp('searchCon').getValue();
    var search_text = Ext.getCmp('search_text').getValue();
    if (Ext.getCmp('dateCon').getValue() != "0") {
        if (Ext.getCmp('timestart').getValue() == ("" || null)) {
            Ext.Msg.alert(INFORMATION, "請選擇查詢日期");
            return;
        }
        if (Ext.getCmp('timeend').getValue() == ("" || null)) {
            Ext.Msg.alert(INFORMATION, "請選擇查詢日期");
            return;
        }
    }
    EpaperContentStore.removeAll();
    Ext.getCmp("gdEpaper").store.loadPage(1, {
        params: {
            searchCon: searchCon,
            search_text: search_text,
            dateCon: Ext.getCmp('dateCon').getValue(),
            date_start: Ext.getCmp('timestart').getValue(),
            date_end: Ext.getCmp('timeend').getValue(),
            activeStatus: Ext.getCmp('activeStatus').getValue(),
            sizeCon: Ext.getCmp('sizeCon').getValue()
        }
    });

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


function TranToDetial(url, epaper_id) {

    var urlTran = url + '?epaperId=' + epaper_id;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#detial');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'detial',
        title: '活動頁面歷史記錄',
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();

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
//function formatDate(now) {
//    var year = now.getFullYear();
//    var month = now.getMonth() + 1;
//    var date = now.getDate();
//    var hour = now.getHours();
//    var minute = now.getMinutes();
//    var second = now.getSeconds();
//    return year + "-" + month + "-" + date + "   " + hour + ":" + minute + ":" + second;
//};
function Today() {
    var d;
    d = new Date();                             // 创建 Date 对象。
    d.setDate(d.getDate() + 1);
    return d;
}
//時間
function formatDate() {
    var d;
    d = new Date();                             // 创建 Date 对象。
    d.setDate(d.getDate() + 1);
    return d;
}
function Tomorrow() {
    var d;
    d = new Date();                             // 创建 Date 对象。
    d.setDate(d.getDate() + 1);
    return d;
}
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
