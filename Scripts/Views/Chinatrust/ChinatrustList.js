var CallidForm;
var pageSize = 25;
Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 25;
//促銷贈品表Model
Ext.define('gigade.EventChinatrust', {
    extend: 'Ext.data.Model',
    fields: [
         { name: "row_id", type: "int" },//流水號
        { name: "event_type", type: "string" },//活動類型
        { name: "event_id", type: "string" },//活動編號
        { name: "event_name", type: "string" },//活動名稱
        { name: "event_desc", type: "string" },//活動描述
        { name: "event_banner", type: "string" },//活動-banner
        { name: "s_event_banner", type: "string" },//活動-banner圖片
        { name: "event_start_time", type: "string" },//活動開始時間
        { name: "event_end_time", type: "string" },//活動結束時間
        { name: "event_active", type: "int" },//活動狀態
        { name: "create_name", type: "string" },//創建人
        { name: "update_name", type: "string" },//修改人
        { name: "event_create_time", type: "string" },//創建時間
        { name: "event_update_time", type: "string" },//修改時間
        { name: "user_register_time", type: "string" }//會員註冊日期

    ]
});
var EventChinatrustStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.EventChinatrust',
    proxy: {
        type: 'ajax',
        url: '/Chinatrust/GetChinatrustList',
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
            var row = Ext.getCmp("EventChinatrust").getSelectionModel().getSelection();
            Ext.getCmp("EventChinatrust").down('#edit').setDisabled(selections.length == 0);
            //Ext.getCmp("EventChinatrust").down('#delete').setDisabled(selections.length == 0);
        }
    }
});
var DDLStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "全部", "value": "-1" },
        { "txt": "啟用", "value": "1" },
        { "txt": "禁用", "value": "0" }
    ]
});
var DateStore = Ext.create('Ext.data.Store', {
    fields: ['date', 'dvalue'],
    data: [
        { "date": "全部日期", "dvalue": "0" },
        { "date": "活動開始時間", "dvalue": "1" },
        { "date": "活動結束時間", "dvalue": "2" },
        { "date": "修改時間", "dvalue": "3" }
    ]
});
EventChinatrustStore.on('beforeload', function () {

    Ext.apply(EventChinatrustStore.proxy.extraParams, {
        event_active: Ext.getCmp('ddlSel').getValue(),
        event_search: Ext.getCmp('event_id_name').getValue(),
        TimeStart: Ext.getCmp('start_time').getValue(),//課程開始時間
        TimeEnd: Ext.getCmp('end_time').getValue(),//課程結束時間
        date: Ext.getCmp('date').getValue()
    })

});
function Query(x) {
    EventChinatrustStore.removeAll();
    if ((Ext.getCmp('start_time').getValue() == null && Ext.getCmp('end_time').getValue() == null) && Ext.getCmp('date').getValue() != 0)
    {
        Ext.Msg.alert(INFORMATION, '請選擇日期');
    }
    else if ((Ext.getCmp('start_time').getValue() != null || Ext.getCmp('end_time').getValue() != null) && Ext.getCmp('date').getValue() == 0) {
        Ext.Msg.alert(INFORMATION, '請選擇日期條件');
    }
    else {
        Ext.getCmp("EventChinatrust").store.loadPage(1);
    }
}

Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 120,
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
                               xtype: 'textfield',
                               id: 'event_id_name',
                               margin: '0 5px',
                               name: 'event_id_name',
                               fieldLabel: '活動編號/名稱',
                               labelWidth: 90,
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
                               editable: false,
                               fieldLabel: "狀態",
                               labelWidth: 30,
                               id: 'ddlSel',
                               store: DDLStore,
                               displayField: 'txt',
                               valueField: 'value',
                               value: '-1'
                           }

                   ]
               },
             {
                 xtype: 'fieldcontainer',
                 combineErrors: true,
                 layout: 'hbox',
                 items: [
                       //{
                       //    xtype: 'label',
                       //    width: 74,
                       //    margin: '1 5 0 5 ',
                       //    text: '活動時間:'

                       //},
                       {
                           xtype: 'combobox',
                           editable: false,
                           fieldLabel: "日期條件",
                           labelWidth: 90,
                           width: 220,
                           margin: '0 5 0 5 ',
                           id: 'date',
                           store: DateStore,
                           displayField: 'date',
                           valueField: 'dvalue',
                           value: '0'
                           //emptyText:'請選擇'
                       },
                       {
                           xtype: "datefield",
                           editable: false,
                           margin: '0 0 0 5',
                           id: 'start_time',
                           name: 'start_time',
                           format: 'Y-m-d',
                           width: 110,
                          // value: Tomorrow(1 - new Date().getDate()),
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
                           value: '~ '
                       },
                       {

                           xtype: "datefield",
                           editable: false,
                           id: 'end_time',
                           name: 'end_time',
                           format: 'Y-m-d',
                           //value: Tomorrow(0),
                           width: 110,
                           listeners: {
                               select: function (a, b, c) {
                                   var start = Ext.getCmp("start_time");
                                   var end = Ext.getCmp("end_time");
                                   var s_date = new Date(end.getValue());
                                   if (start.getValue() == null)
                                   {
                                       Ext.getCmp("start_time").setValue(new Date(s_date.setMonth(s_date.getMonth() - 1)));
                                   }
                                   /*搜索條件限制在一個月之內
                                   var data1 = Date.parse(start.getValue());
                                   var data2 = Date.parse(end.getValue());
                                   var datadiff = data2 - data1;
                                   var time = 31 * 24 * 60 * 60 * 1000;*/
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
                       }



                 ]
             },
              {
                  xtype: 'fieldcontainer',
                  combineErrors: true,
                  layout: 'hbox',
                  margin: '10 0 0 0',
                  items: [
                      {
                          xtype: 'button',
                          margin: '0 8 0 8',
                          iconCls: 'icon-search',
                          text: "查詢",
                          handler: Query
                      },
                      {
                          xtype: 'button',
                          text: '重置',
                          id: 'btn_reset',
                          iconCls: 'ui-icon ui-icon-reset',
                          listeners: {
                              click: function () {
                                  Ext.getCmp('event_id_name').setValue("");//訂單 / 細項 / 課程編號
                                  Ext.getCmp('ddlSel').setValue("-1");//狀態
                                  Ext.getCmp('start_time').reset();//開始時間--time_start--delivery_date
                                  Ext.getCmp('end_time').reset();//結束時間--time_end--delivery_date
                                  Ext.getCmp('date').reset();
                              }
                          }
                      }
                  ]
              }
        ]
    });
    var EventChinatrust = Ext.create('Ext.grid.Panel', {
        id: 'EventChinatrust',
        store: EventChinatrustStore,
        flex: 9.4,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
                        { header: "流水號", dataIndex: 'row_id', width: 50, align: 'center' },
                        { header: "活動類型", dataIndex: 'event_type', width: 120, align: 'center' },
                        {
                            header: "活動編號", dataIndex: 'event_id', width: 70, align: 'center',
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                    return "<a href='javascript:void(0);' onclick='TranToEventChinatrustBag(\"" + record.data.event_id + "\")'>" + value + "</a>";//<span style='color:Red;'></span>
                            }
                        },
                        { header: "活動名稱", dataIndex: 'event_name', width: 150, align: 'center' },
                        { header: "活動描述", dataIndex: 'event_desc', width: 150, align: 'center' },
                        { header: "banner", dataIndex: 'event_banner', width: 120, align: 'center',hidden:true },
                        {
                            header: "活動-banner", dataIndex: 's_event_banner', width: 150, align: 'center', width: 100,
                             xtype: 'templatecolumn',
                             tpl: '<a target="_blank" href="{s_event_banner}" ><img width=50 name="tplImg"  height=50 src="{s_event_banner}" /></a>'
                         },
                        { header: "活動開始時間", dataIndex: 'event_start_time', width: 120, align: 'center' },
                        { header: "活動結束時間", dataIndex: 'event_end_time', width: 120, align: 'center' },
                        { header: "修改人", dataIndex: 'update_name', width: 90, align: 'center' },
                        { header: "修改時間", dataIndex: 'event_update_time', width: 120, align: 'center' },
                         {
                             header: "活動狀態",
                             dataIndex: 'event_active',
                             id: 'status',
                             align: 'center',
                             renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                 if (value == 1) {
                                     return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.row_id + ")'><img hidValue='0' id='img" + record.data.row_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                                 } else {
                                     return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.row_id + ")'><img hidValue='1' id='img" + record.data.row_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                                 }
                             }
                         }
        ],
        tbar: [
            { xtype: 'button', text: "新增", id: 'add', iconCls: 'ui-icon ui-icon-user-add', handler: onAddClick },
            { xtype: 'button', text: "編輯", id: 'edit', iconCls: 'ui-icon ui-icon-user-edit', disabled: true, handler: onEditClick }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: EventChinatrustStore,
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
        }
          , selModel: sm
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [frm, EventChinatrust],//
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                EventChinatrust.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //EventChinatrustStore.load({ params: { start: 0, limit: 25 } });
});
function TranToEventChinatrustBag(eventId) {
    var url = '/Chinatrust/ChinatrustBag?eventId=' + eventId;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#EventChinatrustBag');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'EventChinatrustBag',
        title: '中信區域包',
        // html: window.top.rtnFrame(url),
        //html: '<iframe width=100% height=100% src=' + url + '/>',]
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}

//*********新增********//
onAddClick = function () {
    editFunction(null, EventChinatrustStore);
}

function Tomorrow(s) {
    var d;
    d = new Date();                             // 创建 Date 对象。                               // 返回日期。
    d.setDate(d.getDate() + s);
    return d;
}


//*********編輯********//
onEditClick = function () {
    var row = Ext.getCmp("EventChinatrust").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert("提示", "未選中任何行!");
    }
    else if (row.length > 1) {
        Ext.Msg.alert("提示", "只能選擇一行!");
    } else {
        editFunction(row[0], EventChinatrustStore);
    }
}

function Today() {
    var d;
    d = new Date();                             // 创建 Date 对象。                               // 返回日期。
    d.setSeconds(00);
    d.setMinutes(00);
    d.setHours(00);
    //d.setDate(d.getDate() + 1+s);
    return d;
}
//更改活動狀態(啟用前先檢查該活動是否具有有效贈品，是則啟用，否則提示請設定有效贈品)
function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");

    $.ajax({
        url: "/Chinatrust/UpdateActiveQuestion",
        data: {
            "row_id": id,
            "event_active": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            EventChinatrustStore.load();
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

