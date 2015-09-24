var pageSize = 25;
Ext.define('gigade.EdmContentNew', {
    extend: 'Ext.data.Model',
    fields: [
    { name: "content_id", type: "int" },
    { name: "group_id", type: "int" },
    { name: "subject", type: "string" },
    { name: "template_id", type: "int" },
    { name: "template_data", type: "string" },
    { name: "importance", type: "int" },
    { name: "sender_id", type: "int" },
    { name: "content_createdate", type: "string" },
    { name: "content_updatedate", type: "string" },
    { name: "content_create_userid", type: "int" },
    { name: "content_update_userid", type: "int" },
    { name: "schedule_date", type: "string" },
    { name: "count", type: "int" },
    { name: "date", type: "string" },
    ]
});
EdmContentNewStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.EdmContentNew',
    proxy: {
        type: 'ajax',
        url: '/EdmNew/GetECNList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

EdmContentNewStore.on('beforeload', function () {
    Ext.apply(EdmContentNewStore.proxy.extraParams,
    {
       
    });
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("EdmContentNew").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("EdmContentNew").down('#report').setDisabled(selections.length == 0);
            Ext.getCmp("EdmContentNew").down('#goSend').setDisabled(selections.length == 0);

            
        }
    }
});
//var channelTpl = new Ext.XTemplate(
//'<a href="JavaScript:void{0}" >報表</a>'
//);

//var EditTpl = new Ext.XTemplate(
//'<a href=javascript:TranToDetial("/Edm/EdmContentAdd","{content_id}")>' + "修改" + '</a> '
//)

Ext.onReady(function () {
   
    var EdmContentNew = Ext.create('Ext.grid.Panel', {
        id: 'EdmContentNew',
        store: EdmContentNewStore,
        flex:'8.9',
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
        { header: "編號", dataIndex: 'content_id', width: 60, align: 'center' },
        { header: "正式發送", dataIndex: 'count', width: 150, align: 'center' },
        { header: "郵件主旨", dataIndex: 'subject', width: 200, align: 'center' },
        {
            header: "發送時間", dataIndex: 'date', width: 150, align: 'center'
            , renderer: function (value) {
                if (value == "0001-01-01 00:00:00") {
                    return "";
                }
                else {
                    return value;
                }
            }
        },
        ],
        tbar: [
        { xtype: 'button', text:'新增', id: 'add', hidden: false, iconCls: 'icon-user-add',   handler: onAddClick },
        { xtype: 'button', text: '編輯', id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
        { xtype: 'button', text: "前往發送", id: 'goSend', hidden: false,disabled: true,handler: onGoSendClick  },
        { xtype: 'button', text: "報表", id: 'report', hidden: false, disabled: true, },
 
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: EdmContentNewStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: "當前顯示記錄" + ': {0} - {1}' + "共計" + ': {2}',
            emptyMsg: "沒有記錄可以顯示"
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
        items: [EdmContentNew],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                EdmContentNew.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //EdmContentStore.load({ params: { start: 0, limit: 25 } });
});

/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {
    editFunction(null, EdmContentNewStore);
}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("EdmContentNew").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert("提示信息", "沒有選擇一行");
    } else if (row.length > 1) {
        Ext.Msg.alert("提示信息", "只能選擇一行");
    } else if (row.length == 1) {
        editFunction(row[0], EdmContentNewStore);
    }
}
onGoSendClick = function () {
    var row = Ext.getCmp("EdmContentNew").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert("提示信息", "沒有選擇一行");
    } else if (row.length > 1) {
        Ext.Msg.alert("提示信息", "只能選擇一行");
    } else if (row.length == 1) {
        sendFunction(row[0], EdmContentNewStore);
    }
}
onStatusClick = function () {
    var row = Ext.getCmp("EdmContentNew").getSelectionModel().getSelection();
    statusFunction(row[0], EdmContentNewStore);
}
//******刪除****///
onRemoveClick = function () {
    //var row = Ext.getCmp("gdSites").getSelectionModel().getSelection();
    //if (row.length == 0) {
    //    Ext.Msg.alert("提示信息", "未選中一行");
    //}
    //else {
    //    Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
    //        if (btn == 'yes') {
    //            var rowIDs = '';
    //            for (var i = 0; i < row.length; i++) {
    //                rowIDs += row[i].data.content_id + '|';
    //            }
    //            Ext.Ajax.request({
    //                url: '/Edm/DeleteEdm',
    //                method: 'post',
    //                params: { rowID: rowIDs },
    //                success: function (form, action) {
    //                    var result = Ext.decode(form.responseText);
    //                    if (result.success) {
    //                        Ext.Msg.alert("提示信息", "刪除成功");
    //                        EdmContentStore.load();
    //                    }
    //                    else {
    //                        Ext.Msg.alert("提示信息", "刪除失敗");
    //                        EdmContentStore.load();
    //                    }
    //                }
    //            });
    //        }
    //    });
    //}
}





