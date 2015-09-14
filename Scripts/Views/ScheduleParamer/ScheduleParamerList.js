
Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 25;
//聲明grid
Ext.define('GIGADE.ScheduleParameter', {
    extend: 'Ext.data.Model',
    fields: [
              { name: "para_id", type: "int" },
              { name: "para_value", type: "string" },
              { name: "para_name", type: "string" },
              { name: "para_status", type: "string" },
              { name: "schedule_code", type: "string" }

    ]
});
//獲取grid中的數據
var ScheduleParameterStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'GIGADE.ScheduleParameter',
    proxy: {
        type: 'ajax',
        url: '/ScheduleParamer/ScheduleParamerList',
        actionMethods: 'post',
        reader: {
            type: 'json',
            totalProperty: 'totalCount',
            root: 'data'
        }
    }
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("ScheduleParameterGrid").down('#Remove').setDisabled(selections.length == 0);
            Ext.getCmp("ScheduleParameterGrid").down('#Edit').setDisabled(selections.length == 0)

        }
    }
});

//加載前先獲取ddl的值
ScheduleParameterStore.on('beforeload', function () {
    Ext.apply(ScheduleParameterStore.proxy.extraParams, {
    })

});


Ext.onReady(function () {

    //頁面加載時創建grid
    var ScheduleParameterGrid = Ext.create('Ext.grid.Panel', {
        id: 'ScheduleParameterGrid',
        store: ScheduleParameterStore,
        columnLines: true,
        flex: 1.8,
        frame: true,
        columns: [
            {
                header: "id", dataIndex: 'para_id', width: 100, align: 'center'
            },
            {
                header: "參數值", dataIndex: 'para_value', width: 150, align: 'center'
            },
            { header: "參數名稱", dataIndex: 'para_name', width: 120, align: 'center' },
            { header: "明細代碼", dataIndex: 'schedule_code', width: 100, align: 'center' },
            {
                header: "參數狀態", dataIndex: 'para_status', width: 180, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 1) {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.para_id  + ")'><img hidValue='0' id='img" + record.data.para_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                    } else {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.para_id  + ")'><img hidValue='1' id='img" + record.data.para_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                    }
                }
            }
        ],
        tbar: [
            { xtype: 'button', id: 'Add', text: ADD, iconCls: 'icon-add', handler: onAddClick },
            { xtype: 'button', id: 'Edit', text: EDIT, iconCls: 'icon-edit', disabled: true, handler: onEditClick },
            { xtype: 'button', id: 'Remove', text: REMOVE, iconCls: 'icon-remove', disabled: true, handler: onRemoveClick }

        ],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        bbar: Ext.create('Ext.PagingToolbar', {
            store: ScheduleParameterStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }),
        selModel: sm
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [ScheduleParameterGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                ScheduleParameterGrid.width = document.documentElement.clientWidth;
                this.doLayout();

            }
        }
    });

    ToolAuthority();
    ScheduleParameterStore.load({ params: { start: 0, limit: 25 } });

})

//更改活動狀態(設置活動可用與不可用)
function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/ScheduleParamer/UpdateActive",
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
            } else {
                $("#img" + id).attr("hidValue", 1);
                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
            }
            ScheduleParameterStore.load(1);
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);
        }
    });
}



onAddClick = function () {
    editFunction(null, ScheduleParameterStore);
}



onEditClick = function () {
    var row = Ext.getCmp("ScheduleParameterGrid").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], ScheduleParameterStore);
    }


}


onRemoveClick = function () {
    var row = Ext.getCmp("ScheduleParameterGrid").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.para_id + '|';
                }
                Ext.Ajax.request({
                    url: '/ScheduleParamer/DeleteScheduleParameter',
                    method: 'post',
                    params: { rowID: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            Ext.Msg.alert(INFORMATION, SUCCESS);
                            ScheduleParameterStore.load(1);
                        }
                        else {
                            if (result.msg != '')
                            {
                                Ext.Msg.alert(INFORMATION, "刪除第"+result.msg+"條數據時出錯!");
                            }
                        }
                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                    }
                });
            }
        });
    }
}
