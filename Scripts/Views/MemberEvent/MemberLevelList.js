var pageSize = 25;
//群組管理Model
Ext.define('gigade.MemberLevel', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "rowID", type: "int" },
        { name: "ml_code", type: "string" },
        { name: "ml_name", type: "string" },
        { name: "ml_seq", type: "int" },
        { name: "ml_minimal_amount", type: "int" },
        { name: "ml_max_amount", type: "int" },
        { name: "ml_month_seniority", type: "int" },
        { name: "ml_last_purchase", type: "int" },
        { name: "ml_minpurchase_times", type: "int" },
        { name: "ml_birthday_voucher", type: "int" },
        { name: "ml_shipping_voucher", type: "int" },
        { name: "ml_status", type: "int" },
        { name: 'k_date', type: 'string' },
        { name: 'm_date', type: 'int' },
        { name: "m_date", type: "string" },
        { name: "m_user", type: "int" }
    ]
});


var MLStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "全部", "value": "-1" },
        { "txt": "啟用", "value": "1" },
        { "txt": "禁用", "value": "0" }
    ]
});
var MemberLevelStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.MemberLevel',
    proxy: {
        type: 'ajax',
        url: '/MemberEvent/MemberLevelList',
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
            Ext.getCmp("MemberLevel").down('#ml_edit_level').setDisabled(selections.length == 0);
        }
    }
});

MemberLevelStore.on('beforeload', function () {
    Ext.apply(MemberLevelStore.proxy.extraParams,
        {
            code_name: Ext.getCmp('code_name').getValue(),
            mem_status: Ext.getCmp('mem_status').getValue(),
        });
});
Ext.onReady(function () {

    var searchform = Ext.create('Ext.form.Panel', {
        id: 'searchform',
        height: 100,
        bodyPadding: 10,
        border: false,
        items: [
            {
                xtype: 'fieldcontainer',
                // layout: 'hbox',
                items: [
                    {
                        xtype: 'textfield',
                        id: 'code_name',
                        name: 'code_name',
                        fieldLabel: '級別代碼/名稱',
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
                        store: MLStore,
                        displayField: 'txt',
                        valueField: 'value',
                        id: 'mem_status',
                        name: 'mem_status',
                        editable: false,
                        emptyText: '請選擇...',
                        fieldLabel: '是否啟用',
                        value: '-1',
                    },
                ],
            },
        ],
        buttons: [
            {
                text: '查詢',
                iconCls: 'icon-search',
                handler: Query,
            },
             {
                 iconCls: 'ui-icon ui-icon-reset',
                 text: '重置',
                 handler: function () {
                     this.up('form').getForm().reset();
                 }
             },
        ]

    });

    var MemberLevel = Ext.create('Ext.grid.Panel', {
        id: 'MemberLevel',
        store: MemberLevelStore,
        width: document.documentElement.clientWidth,
        height: document.documentElement.clientHeight,
        columnLines: true,
        frame: true,
        //  height:600,
        // flex: 9.4,
        columns: [
            {
                header: "編號", dataIndex: 'rowID', width: 90, align: 'center',
            },
            {
                header: "排序", dataIndex: 'ml_seq', width: 100, align: 'center'
            },
            {
                header: "級別代碼", dataIndex: 'ml_code', width: 60, align: 'center'
            },
            { header: '級別名稱', dataIndex: 'ml_name', width: 150, align: 'center', },
            {
                header: "累積購物金額下限", dataIndex: 'ml_minimal_amount', width: 150, align: 'center'
            },
               {
                   header: "累積購物金額上限", dataIndex: 'ml_max_amount', width: 150, align: 'center'
               },
            {
                header: "加入會員時間",
                // dataIndex: 'ml_month_seniority',
                width: 150, align: 'center'
            },
            {
                header: "前次購買時間",
                // dataIndex: 'ml_last_purchase',
                width: 150, align: 'center',
            },
            {
                header: "最低消費次數",
                //dataIndex: 'ml_minpurchase_times',
                width: 150, align: 'center',
            },
            {
                header: "生日禮金", dataIndex: 'ml_birthday_voucher', width: 150, align: 'center',
            },
              {
                  header: "免運劵數量", dataIndex: 'ml_shipping_voucher', width: 150, align: 'center',
              },
            
            {
                header: "是否啟用", dataIndex: 'ml_status', width: 60, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 1) {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.rowID + ")'><img hidValue='0'  id='img" + record.data.rowID + "' src='../../../Content/img/icons/accept.gif'/></a>";
                    } else {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.rowID + ")'><img hidValue='1'  id='img" + record.data.rowID + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                    }
                }
            }
        ],
        tbar: [
           {
               xtype: 'button', text: ADD, id: 'ml_add_level',
               hidden: true,
               iconCls: 'icon-user-add', handler: onAddClick
           },
           {
               xtype: 'button', text: EDIT, id: 'ml_edit_level',
               hidden: true,
               iconCls: 'icon-user-edit', disabled: true, handler: onEditClick
           },
           '->',
                   {
                       xtype: 'textfield',
                       id: 'code_name',
                       name: 'code_name',
                       fieldLabel: '級別代碼/名稱',
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
                       store: MLStore,
                       margin: '0 5 0 5',
                       displayField: 'txt',
                       valueField: 'value',
                       id: 'mem_status',
                       name: 'mem_status',
                       editable: false,
                       labelWidth: 65,
                       emptyText: '請選擇...',
                       fieldLabel: '是否啟用',
                       value: '-1',
                   },
                     {
                         text: '查詢',
                         iconCls: 'icon-search',
                         handler: Query,
                     },
                       {
                           iconCls: 'ui-icon ui-icon-reset',
                           text: '重置',
                           handler: function () {
                               Ext.getCmp('mem_status').setValue("-1");
                               Ext.getCmp('code_name').setValue("");
                               // this.up('form').getForm().reset();
                           }
                       },


        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: MemberLevelStore,
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
        items: [MemberLevel],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                MemberLevel.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    MemberLevelStore.load({ params: { start: 0, limit: 25 } });
});

/*************************************************************************************新增

*************************************************************************************************/
onAddClick = function () {
    editFunction(null, MemberLevelStore);
}

/*************************************************************************************編輯

*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("MemberLevel").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], MemberLevelStore);
    }
}
/*************************************************************************************更改狀態

*************************************************************************************************/
function UpdateActive(rowID) {
    var ml_status = $("#img" + rowID).attr("hidValue");
    $.ajax({
        url: "/MemberEvent/UpdateActive",
        data: {
            "rowID": rowID,
            "ml_status": ml_status
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            MemberLevelStore.load();
            if (ml_status == 1) {
                $("#img" + rowID).attr("hidValue", 0);
                $("#img" + rowID).attr("src", "../../../Content/img/icons/accept.gif");
            } else {
                $("#img" + rowID).attr("hidValue", 1);
                $("#img" + rowID).attr("src", "../../../Content/img/icons/drop-no.gif");
            }
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);
        }
    });
}
function Query() {
    Ext.getCmp('MemberLevel').store.loadPage(1, {
        params: {
            code_name: Ext.getCmp('code_name').getValue(),
            mem_status: Ext.getCmp('mem_status').getValue(),
        }
    });
}







