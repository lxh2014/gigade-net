var pageSize = 25;
/**********************************************************************群組管理主頁面**************************************************************************************/
//var searchStore = Ext.create('Ext.data.Store', {
//    fields: ['txt', 'value'],
//    data: [
//          { 'txt': '全部狀態', 'value': '-1' },
//          { 'txt': '新建立', 'value': '0' },
//          { 'txt': '顯示', 'value': '1' },
//          { 'txt': '隱藏', 'value': '2' },
//          { 'txt': '下檔', 'value': '3' }
//    ]

//});
var dateStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
           { 'txt': '所有日期', 'value': '0' },
           { 'txt': '活動開始時間', 'value': '1' },
           { 'txt': '活動結束時間', 'value': '2' },
           { 'txt': '顯示開始時間', 'value': '3' },
           { 'txt': '顯示結束時間', 'value': '4' },
    ]
});
//群組管理Model
Ext.define('gigade.ChinaTrustBag', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "bag_id", type: "int" },
        { name: "bag_name", type: "string" },
        { name: "bag_desc", type: "string" },
        { name: "bag_banner", type: "string" },
        { name: "bag_start_time", type: "string" },
        { name: "bag_end_time", type: "string" },
        { name: "bag_active", type: "int" },
        { name: "bag_create_user", type: "int" },
        { name: "bag_update_user", type: "int" },
        { name: "bag_create_time", type: "string" },
        { name: 'bag_update_time', type: 'string' },
        { name: 'bag_show_start_time', type: 'string' },
        { name: "bag_show_end_time", type: "string" },
        { name: "event_id", type: "string" },
        { name: "product_number", type: "int" },
        { name: "create_user", type: "string" },
        { name: "update_user", type: "string" },
        { name: "start_time", type: "string" },
        { name: "end_time", type: "string" },
        { name: "show_start_time", type: "string" },
        { name: "show_end_time", type: "string" },
        { name: "create_time", type: "string" },
        { name: "update_time", type: "string" },
        { name: "s_bag_banner", type: "string" }        
    ]
});


var StatusStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "全部", "value": "-1" },
        { "txt": "啟用", "value": "1" },
        { "txt": "禁用", "value": "0" }
    ]
});
var ChinaTrustBagStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.ChinaTrustBag',
    proxy: {
        type: 'ajax',
        url: '/Chinatrust/EventChinaTrustBagList',
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
            Ext.getCmp("ChinaTrustBag").down('#edit').setDisabled(selections.length == 0);
        }
    }
});

ChinaTrustBagStore.on('beforeload', function () {
    Ext.apply(ChinaTrustBagStore.proxy.extraParams,
        {
            eventId: document.getElementById("eventId").value,
            bag_status: Ext.getCmp('bag_status').getValue(),
            bag_search_name: Ext.getCmp('bag_search_name').getValue(),
            date: Ext.getCmp('date').getValue(),
            start_time: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('start_time').getValue()), 'Y-m-d H:i:s')),
            end_time: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('end_time').getValue()), 'Y-m-d H:i:s')),
        });
});

var EditTpl = new Ext.XTemplate(
        '<a href=javascript:TranToDetial("/Chinatrust/ChinatrustBagMap","{bag_id}")>' + "記錄" + '</a> '
    );

Ext.onReady(function () {
    var searchForm = Ext.create('Ext.form.Panel', {
        id: 'searchForm',
        layout: 'anchor',
        border: 0,
        bodyPadding: 10,
        width: document.documentElement.clientWidth,
        items: [
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                    {
                        xtype: 'textfield',
                        id: 'bag_search_name',
                        fieldLabel: '區域包名稱',
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == Ext.EventObject.ENTER) {
                                    Query();
                                }
                            }
                        }
                    },
                    {
                        xtype: 'combobox',
                        fieldLabel: '狀態',
                        margin: '0 0 0 20',
                        labelWidth:40,
                        id: 'bag_status',
                        store: StatusStore,
                        displayField: 'txt',
                        valueField: 'value',
                        editable:false,
                        value:-1,
                    }
                ]
            },    
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                    {
                        xtype: 'combobox',
                        id: 'date',
                        store: dateStore,
                        fieldLabel: '查詢日期',
                        editable: false,
                        width: 220,
                        displayField: 'txt',
                        valueField: 'value',
                        value: '0',
                    },
                    {
                        xtype: 'datefield',
                        margin: '0 0 0 10',
                      
                        id: 'start_time',
                        format: 'Y-m-d',
                        width: 110,
                        //value: Tomorrow(1 - new Date().getDate()),
                        editable: false,
                        listeners: {
                            select: function (a, b, c) {
                                var start = Ext.getCmp("start_time");
                                var end = Ext.getCmp("end_time");
                                var s_date = new Date(start.getValue());
                                end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                                /*
                                var data1 = Date.parse(start.getValue());
                                var data2 = Date.parse(end.getValue());
                                var datadiff = data2 - data1;
                                var time = 31 * 24 * 60 * 60 * 1000;
                                if (datadiff < 0 || datadiff > time) {
                                    Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                                    end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                                }
                                else {
                                    end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                                }*/
                            },
                            specialkey: function (field, e) {
                                if (e.getKey() == Ext.EventObject.ENTER) {
                                    Query();
                                }
                            }
                        }
                    },
                    {
                        xtype: 'displayfield',
                        margin: '0 5 0 5',
                        value:'~',
                    },
                    {
                        xtype: 'datefield',
                        id: 'end_time',
                        format: 'Y-m-d',
                        width:110,
                        //value: Tomorrow(0),
                        editable: false,
                        listeners: {
                            select: function (a, b, c) {
                                var start = Ext.getCmp("start_time");
                                var end = Ext.getCmp("end_time");                                
                                var s_date = new Date(end.getValue());
                                if (start.getValue() == null) {
                                    Ext.getCmp("start_time").setValue(new Date(s_date.setMonth(s_date.getMonth() - 1)));
                                }
                                //var data1 = Date.parse(start.getValue());
                                //var data2 = Date.parse(end.getValue());
                                //var datadiff = data2 - data1;
                                //var time = 31 * 24 * 60 * 60 * 1000;
                                if (end.getValue() < start.getValue()) {
                                    Ext.Msg.alert("提示", "開始時間不能大於結束時間！");
                                    end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                                }
                                //else if (datadiff < 0 || datadiff > time) {
                                //    Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                                //    end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                                //}
                            },
                            specialkey: function (field, e) {
                                if (e.getKey() == Ext.EventObject.ENTER) {
                                    Query();
                                }
                            }
                        }
                    },
                ]
            }
        ],
        buttonAlign:'left',
        buttons: [
            {
                text: '查詢',
                margin: '0 8 0 8',
                iconCls: 'icon-search',
                handler:function(){
                    Query();
                } 
            },
            {
                text: '重置',
                iconCls: 'ui-icon ui-icon-reset',
                handler: function () {
                    this.up('form').getForm().reset();
                }
            },
        ]
    });
    var ChinaTrustBag = Ext.create('Ext.grid.Panel', {
        id: 'ChinaTrustBag',
        store: ChinaTrustBagStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex: 9.4,
        columns: [
            {
                header: "區域包編號", dataIndex: 'bag_id', width: 90, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return "<a href='javascript:void(0);' onclick='TranToDetial(\"" + record.data.bag_id + "\")'>" + value + "</a>";//<span style='color:Red;'></span>
                }
            },
            {
                header: "活動編號", dataIndex: 'event_id', width: 100, align: 'center'
            },
            {
                header: "商品數量", dataIndex: 'product_number', width: 60, align: 'center'
            },
            { header: '區域包名稱', dataIndex: 'bag_name', width: 150, align: 'center', },
            {
                header: "區域包描述", dataIndex: 'bag_desc', width: 150, align: 'center'
            },
            {
                header: "圖片", dataIndex: 'bag_banner', width: 100, align: 'center',
                //bag_banner
                xtype: 'templatecolumn',
                tpl: '<a target="_blank" href="{s_bag_banner}" ><img width=50 name="tplImg"  height=50 src="{s_bag_banner}" /></a>'
            },
            { header: "開始時間", dataIndex: 'start_time', width: 150, align: 'center' },
            {
                header: "結束時間", dataIndex: 'end_time', width: 150, align: 'center',
            },
            {
                header: "顯示時間開始", dataIndex: 'show_start_time', width: 150, align: 'center',
            },
            {
                header: "顯示時間結束", dataIndex: 'show_end_time', width: 150, align: 'center',
            },
            {
                header: "狀態", dataIndex: 'bag_active', width: 60, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 1) {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.bag_id + ")'><img hidValue='0' id='img" + record.data.bag_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                    } else {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.bag_id + ")'><img hidValue='1' id='img" + record.data.bag_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                    }
                }
            }
        ],
        tbar: [
           { xtype: 'button', text: ADD, id: 'add', iconCls: 'icon-user-add', handler: onAddClick },
           { xtype: 'button', text: EDIT, id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },

        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: ChinaTrustBagStore,
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
        items: [searchForm,ChinaTrustBag],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                ChinaTrustBag.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //ChinaTrustBagStore.load({ params: { start: 0, limit: 25 } });
});

/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {
    editFunction(null, ChinaTrustBagStore);
}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("ChinaTrustBag").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], ChinaTrustBagStore);
    }
}
/*************************************************************************************更改狀態*************************************************************************************************/
function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/Chinatrust/UpdateActive",
        data: {
            "bag_id": id,
            "bag_active": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            ChinaTrustBagStore.load();
            if (activeValue == 1) {
                $("#img" + id).attr("hidValue", 0);
                $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
            } else {
                $("#img" + id).attr("hidValue", 1);
                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
            }
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);
        }
    });
}



function Query() {
    ChinaTrustBagStore.removeAll();
    var date = Ext.getCmp('date').getValue();
    var start_time = Ext.getCmp('start_time').getValue() == null ? "" : Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('start_time').getValue()), 'Y-m-d H:i:s'));
    var end_time = Ext.getCmp('end_time').getValue() == null ? "" : Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('end_time').getValue()), 'Y-m-d H:i:s'));
    if (date != 0 && (start_time == "" && end_time == "")) {
        Ext.Msg.alert(INFORMATION, '請選擇日期');
    }
    else if (date == 0 && (start_time != "" && end_time != "")) {
        Ext.Msg.alert(INFORMATION, '請選擇日期條件');
    }
    else {
        Ext.getCmp('ChinaTrustBag').store.loadPage(1, {
            params: {
                bag_status: Ext.getCmp('bag_status').getValue(),
                bag_id_name: Ext.getCmp('bag_search_name').getValue(),
                date: Ext.getCmp('date').getValue(),
                start_time: Ext.getCmp('start_time').getValue() == null ? "" : Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('start_time').getValue()), 'Y-m-d H:i:s')),
                end_time: Ext.getCmp('end_time').getValue() == null ? "" : Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('end_time').getValue()), 'Y-m-d H:i:s'))

            }
        });
    }
}

function TranToDetial(bag_id) {
    var urlTran = '/Chinatrust/ChinatrustBagMap?bag_id=' + bag_id;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#ChinaTrustBagMap');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'ChinaTrustBagMap',
        title: '中信區域包詳情',
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}
function Tomorrow(s) {
    var d;
    d = new Date();                             // 创建 Date 对象。                               // 返回日期。
    d.setDate(d.getDate() + s);
    return d;
}







