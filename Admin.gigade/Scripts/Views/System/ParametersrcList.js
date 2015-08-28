
var pageSize = 25;
/**********************************************************************站臺管理主頁面**************************************************************************************/
//站臺管理Model
Ext.define('gigade.Parametersrc', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "Rowid", type: "int" },
        { name: "ParameterType", type: "string" },
        { name: "ParameterProperty", type: "string" },
        { name: "ParameterCode", type: "string" },
        { name: "parameterName", type: "string" },
        { name: "TopValue", type: "string" },
        { name: "Kdate", type: "string" },
        { name: "Kuser", type: "string" },
        { name: "Used", type: "int" },
        { name: "Sort", type: "int" },
        { name: "remark", type: "string" }

    ]
});

var ParametersrcStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Parametersrc',
    proxy: {
        type: 'ajax',
        url: '/System/ParametersrcList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

ParametersrcStore.on('beforeload', function () {
    Ext.apply(ParametersrcStore.proxy.extraParams,
        {
            serchcontent: Ext.getCmp('serchcontent').getValue()
        });
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("ParametersrcList").down('#edit').setDisabled(selections.length == 0);

        }
    }
});

function Query() {
    if (Ext.getCmp('serchcontent').getValue() != "") {
        ParametersrcStore.removeAll();
        Ext.getCmp("ParametersrcList").store.loadPage(1);
    }
    else {
        Ext.Msg.alert(INFORMATION, SEARCH_LIMIT);
    }
}

Ext.onReady(function () {
    var ParametersrcList = Ext.create('Ext.grid.Panel', {
        id: 'ParametersrcList',
        store: ParametersrcStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "參數ID", dataIndex: 'Rowid', width: 60, align: 'center' },
            { header: "參數類型", dataIndex: 'ParameterType', width: 150, align: 'center' },
            { header: "參數屬性", dataIndex: 'ParameterProperty', width: 100, align: 'center' },
            { header: "參數編碼", dataIndex: 'ParameterCode', width: 100, align: 'center' },
            { header: "參數名稱", dataIndex: 'parameterName', width: 200, align: 'center' },
            { header: "說明", dataIndex: 'remark', width: 200, align: 'center' },
            { header: "父節點", dataIndex: 'TopValue', width: 60, align: 'center' },
            { header: "建立時間", dataIndex: 'Kdate', width: 150, align: 'center' },
            { header: "建立人", dataIndex: 'Kuser', width: 100, align: 'center' },
            { header: "排序", dataIndex: 'Sort', width: 60, align: 'center' },

            {
                header: "狀態", dataIndex: 'Used', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 1) {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.Rowid + ")'><img hidValue='0' id='img" + record.data.Rowid + "' src='../../../Content/img/icons/accept.gif'/></a>";
                    }
                    else {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.Rowid + ")'><img hidValue='1' id='img" + record.data.Rowid + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                    }
                }
            }
        ],
        tbar: [
           { xtype: 'button', text: ADD, id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
           { xtype: 'button', text: EDIT, id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
           '->',
           {
               xtype: 'textfield', allowBlank: true, id: 'serchcontent', name: 'serchcontent', fieldLabel: "關鍵字", labelWidth: 60,
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
               iconCls: 'icon-search',
               id: 'btnQuery',
               handler: Query
           },
          {
              xtype: 'button',
              text: '重置',
              id: 'btn_reset',
              listeners: {
                  click: function () {
                      Ext.getCmp("serchcontent").setValue("");
                  }
              }
          }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: ParametersrcStore,
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
        layout: 'fit',
        items: [ParametersrcList],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                ParametersrcList.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    // ParametersrcStore.load({ params: { start: 0, limit: 25 } });
});

/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {
    editFunction(null, ParametersrcStore);
}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("ParametersrcList").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], ParametersrcStore);
    }
}

//更改會員狀態(啟用或者禁用)
function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/System/UpdateUsed",
        data: {
            "id": id,
            "active": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            if (activeValue == 1) {
                $("#img" + id).attr("hidValue", 0);
                $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
                ParametersrcStore.load();
            }
            else {
                $("#img" + id).attr("hidValue", 1);
                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                ParametersrcStore.load();
            }
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);
            ParametersrcStore.load();
        }
    });
}





