var pageSize = 25;
Ext.define('gigade.TicketDetail', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "course_id", type: "int" },
        { name: "course_name", type: "string" },
        { name: "spec_name_1", type: "string" },
        { name: "spec_name_2", type: "string" },
        { name: "start_date", type: "string" },
        { name: "end_date", type: "string" },
        { name: "ticket_code", type: "string" },
        { name: "ticket_detail_id", type: "string" },
        { name: "flag", type: "int" },
         { name: "ticket_id", type: "int" }
    ]
});
var TicketDetailStore = Ext.create('Ext.data.Store', {
    model: 'gigade.TicketDetail',
    autoDestroy: true,
    pageSize: pageSize,
    proxy: {
        type: 'ajax',
        url: '/Ticket/GetTicketDetail',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});



TicketDetailStore.on('beforeload', function () {
    Ext.apply(TicketDetailStore.proxy.extraParams, {
        select_ticket: Ext.getCmp('select_ticket').getValue(),
        select_code: Ext.getCmp('select_code').getValue(),
        search_status: Ext.getCmp('search_status').getValue()
    });
});


var StatusStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "不分", "value": "-1" },
        { "txt": "未使用", "value": "0" },
        { "txt": "已使用", "value": "1" }
    ]
});
function Query() {
    if (Ext.getCmp('select_ticket').getValue() != "" || Ext.getCmp('select_code').getValue() != "" || Ext.getCmp('search_status').getValue() != "-1") {
        TicketDetailStore.loadPage(1);
    }
    else {
        Ext.Msg.alert(INFORMATION, SEARCH_LIMIT);
    }

}
Ext.onReady(function () {

    var TicketForm = Ext.create('Ext.form.Panel', {
        id: 'TicketForm',
        name: 'form',
        height: 100,
        border: 0,
        defaults: { anchor: '95%', msgTarget: "side" },
        width: document.documentElement.clientWidth,
        items: [
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                    {
                        xtype: 'textfield',
                        fieldLabel: "訂單號",
                        labelWidth: 60,
                        id: 'select_ticket',
                        name: 'select_ticket',
                        margin: '5 5 0 10',
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == e.ENTER) {
                                    Query();
                                }
                            }
                        }
                    },
                       {

                           xtype: 'textfield',
                           fieldLabel: '票券號碼',
                           labelWidth: 60,
                           id: 'select_code',
                           name: 'select_code',
                           margin: '5 5 0 10',
                           listeners: {
                               specialkey: function (field, e) {
                                   if (e.getKey() == e.ENTER) {
                                       Query();
                                   }
                               }
                           }
                       }

                ]
            },
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                      {
                          xtype: 'combobox',
                          name: 'search_status',
                          id: 'search_status',
                          editable: false,
                          fieldLabel: "票券狀態",
                          labelWidth: 60,
                          margin: '5 5 0 10',
                          store: StatusStore,
                          queryMode: 'local',
                          displayField: 'txt',
                          valueField: 'value',
                          value: -1,
                          listeners: {
                              specialkey: function (field, e) {
                                  if (e.getKey() == e.ENTER) {
                                      Query();
                                  }
                              }
                          }
                      },
                       {
                           xtype: 'button',
                           text: '查詢',
                           margin: '5 5 0 10',
                           iconCls: 'icon-search',
                           handler: Query
                       },
                        {
                            xtype: 'button',
                            text: '重置',
                            margin: '5 5 0 10',
                            iconCls: 'ui-icon ui-icon-reset',
                            handler: function () {
                                this.up('form').getForm().reset();
                            }
                        }
                ]
            }
        ],
        buttonAlign: 'left',
        buttons: [

        ]
    });

    var sm = Ext.create('Ext.selection.CheckboxModel', {
        listeners: {
            selectionchange: function (sm, selections) {
                Ext.getCmp("TicketDetailGrid").down('#verification').setDisabled(selections.length == 0);
            }
        }
    });
    var TicketDetailGrid = Ext.create('Ext.grid.Panel', {
        id: 'TicketDetailGrid',
        store: TicketDetailStore,
        flex: '8.5',
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
             {
                 header: "id", dataIndex: 'ticket_id', width: 100, hidden: true, align: 'center', id: 'ticket_id'

             },
                        {
                            header: "訂單號", dataIndex: 'ticket_detail_id', width: 120, align: 'center', id: 'detail_id'

                        },
                        {
                            header: "課程名稱", dataIndex: 'course_name', flex: 1, align: 'center', id: 'course_name'
                        },
                        {
                            header: "規格1", dataIndex: 'spec_name_1', flex: 0.5, align: 'center', id: 'spec_name_1'
                        },
                          {
                              header: "規格2", dataIndex: 'spec_name_2', flex: 0.5, align: 'center', id: 'spec_name_2'
                          },
                        {
                            header: "課程開始時間", dataIndex: 'start_date', width: 135, align: 'center', id: 'startdate'
                        },
                        {
                            header: "課程結束時間", dataIndex: 'end_date', width: 135, align: 'center', id: 'enddate'
                        },
                        {
                            header: "票券號碼", dataIndex: 'ticket_code', width: 180, align: 'center', id: 'ticket_code'
                        },
            {
                header: "核銷",
                dataIndex: 'flag',
                id: 'controlflag',
                //   hidden: true,
                width: 120,
                align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 0) {
                        return "<a href='javascript:void(0);' onclick='Verification(" + record.data.ticket_id + ",1,0)'><img hidValue='0' id='img" + record.data.ticket_id + "' src='../../../Content/img/icons/user_edit.png'/></a>";
                    } else {
                        return "<a href='javascript:void(0);' ><img hidValue='1' id='img" + record.data.ticket_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                    }
                }
            }

        ],
        tbar: [
       { xtype: 'button', text: "核銷", id: 'verification', disabled: true, iconCls: 'icon-user-edit', handler: OnVerify }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: TicketDetailStore,
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
        }, selModel: sm
    });



    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [TicketForm, TicketDetailGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                this.doLayout();
            }
        }
    });
    ToolAuthority();



});
//返回四位隨機整數驗證碼
function returnCode() {
    var rnd = "";
    for (var i = 0; i < 4; i++) {
        rnd += Math.floor(Math.random() * 10) + " ";//獲取0-9的隨機整數
    }
    return rnd;
}



function Verification(ticketId, type, code) {
    var itemFrm;
    if (type == 1) {
        itemFrm = Ext.create('Ext.form.FieldContainer', {
            combineErrors: false,
            layout: 'hbox',
            items: [
                {
                    xtype: 'textfield',
                    fieldLabel: '驗證碼',
                    labelWidth: 45,
                    id: 'check_code',
                    name: 'check_code',
                    margin: '0 5 0 0',
                    allowBlank: false,
                    width: 254,
                    submitValue: true,
                    listeners: {
                        blur: function () {
                            var oVal = Ext.getCmp('check_code');
                            if (oVal.getValue() != "") {
                                var reg = /\s/g;
                                if (document.getElementById("key").innerHTML.replace(reg, '') != oVal.getValue()) {
                                    Ext.Msg.alert(INFORMATION, "驗證碼錯誤！");
                                    oVal.setValue("");
                                    Ext.getCmp('showcode').fireEvent('click');
                                }
                            }
                        }
                        ,
                        specialkey: function (field, e) {
                            if (e.getKey() == e.ENTER) {
                                Ext.getCmp("check").fireEvent('click');
                            }
                        }

                    }
                },
                {
                    xtype: 'button',
                    cls: 'icon-code',
                    name: 'showcode',
                    id: 'showcode',
                    width: 84,
                    margin: '0 0 0 0',
                    submitValue: true,
                    text: "<span id='key' style='font-size:12px;font-weight:bold;font-style:italic;'>" + returnCode() + "</span>",
                    listeners: {
                        click: function () {
                            Ext.getCmp('showcode').setText("<span id='key' style='font-size:12px;font-weight:bold;font-style:italic;'>" + returnCode() + "</span>");
                        }
                    }
                }
            ]
        });
    } else if (type == 2) {
        itemFrm = Ext.create('Ext.form.FieldContainer', {
            combineErrors: false,
            layout: 'hbox',
            items: [
                 {
                     xtype: 'textfield',
                     fieldLabel: '票券號碼',
                     labelWidth: 70,
                     width: 300,
                     id: 'code',
                     name: 'code',
                     allowBlank: false,
                     listeners: {
                         specialkey: function (field, e) {
                             if (e.getKey() == e.ENTER) {
                                 Ext.getCmp("check").fireEvent('click');
                             }
                         }
                     }
                 }
            ]
        });
    }

    var editFlagFrm = Ext.create('Ext.form.Panel', {
        id: 'editFlagFrm',
        frame: true,
        autoScroll: true,
        defaultType: 'textfield',
        layout: 'anchor',
        url: "/Ticket/DoTicketVerification",
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
          itemFrm
        ], 
        buttons: [
            {
                text: "驗證",
                id: 'check',
                formBind: true,
                disabled: true,
                listeners: {
                    click: function () {
                        var form = this.up('form').getForm();
                        if (form.isValid()) {
                            if (ticketId != "") {
                                if (type == 1) {
                                    var oVal = Ext.getCmp('check_code');
                                    if (oVal.getValue() != "") {
                                        var reg = /\s/g;
                                        if (document.getElementById("key").innerHTML.replace(reg, '') != oVal.getValue()) {
                                            Ext.Msg.alert(INFORMATION, "驗證碼錯誤！");
                                            oVal.setValue("");
                                            Ext.getCmp('showcode').fireEvent('click');
                                            return false;
                                        }
                                    }
                                }
                                else if (type == 2) {
                                    var oVal = Ext.getCmp('code');
                                    if (oVal.getValue() != "") {
                                        if (code != oVal.getValue()) {
                                            Ext.Msg.alert(INFORMATION, "票券號碼輸入錯誤！");
                                            oVal.setValue("");
                                            return false;
                                        }
                                    }
                                }
                                form.submit({
                                    params: {
                                        ticketId: ticketId
                                    },
                                    success: function (form, action) {
                                        var result = Ext.decode(action.response.responseText);
                                        if (result.success) {
                                            TicketDetailStore.loadPage(1);
                                            editVeriWin.close();
                                        } else {
                                            Ext.Msg.alert(INFORMATION, FAILURE);
                                        }
                                    },
                                    failure: function () {
                                        Ext.Msg.alert(INFORMATION, FAILURE);
                                    }
                                });
                            }
                        }
                    }
                }
            }
        ]
    });

    var editVeriWin = Ext.create('Ext.window.Window', {
        title: "序號核銷",
        id: 'editVeriWin',
        iconCls: 'icon-user-edit',
        width: 400,
        height: 120,
        layout: 'fit',
        items: [editFlagFrm],
        y: 200,
        closeAction: 'destroy',
        modal: true,
        constrain: true,
        resizable: false,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
            {
                type: 'close',
                qtip: '是否關閉',
                handler: function (event, toolEl, panel) {
                    Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                        if (btn == "yes") {
                            Ext.getCmp('editVeriWin').destroy();
                        } else {
                            return false;
                        }
                    });
                }
            }
        ],
        listeners: {
            'show': function () {
                editFlagFrm.getForm().reset(); //如果是編輯的話
            }
        }
    });
    editVeriWin.show();

}

function OnVerify() {
    var row = Ext.getCmp("TicketDetailGrid").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        Verification(row[0].data.ticket_id, 2, row[0].data.ticket_code)

    }
}

