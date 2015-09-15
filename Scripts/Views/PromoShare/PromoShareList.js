var pageSize = 25;
//自定义VTypes类型，验证日期范围  

Ext.define('gigade.PromoShare', {
    extend: 'Ext.data.Model',
    fields: [
         { name: "promo_id", type: "int" },
         { name: "promo_name", type: "string" },
         { name: "promo_desc", type: "string" },
         { name: "promo_start", type: "string" },
         { name: "promo_end", type: "string" },
         { name: "promo_active", type: "int" },
         { name: "this_promo_id", type: "int" },
           { name: "promo_event_id", type: "string" },
         
    ]
});

var DDLStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "全部", "value": "2" },
        { "txt": "啟用", "value": "1" },
        { "txt": "未啟用", "value": "0" }
    ]
});

var PromoShareStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'gigade.PromoShare',
    proxy: {
        type: 'ajax',
        url: '/PromoShare/PromoShareList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
//加載前先獲取ddl的值
PromoShareStore.on('beforeload', function () {
    Ext.apply(PromoShareStore.proxy.extraParams,
       {
           ddlstatus: Ext.getCmp("ddlstatus").getValue(),
           promo_name_list: Ext.getCmp("promo_name_list").getValue()
           
       });
});
function Query(x) {
    PromoShareStore.removeAll();
    Ext.getCmp("PromoShareGrid").store.loadPage(1, {
        params: {
            ddlstatus: Ext.getCmp("ddlstatus").getValue(),
            promo_name_list: Ext.getCmp("promo_name_list").getValue()
        }
    });
}
function ChongZhiQuery()
{
    Ext.getCmp("ddlstatus").setValue(2);
    Ext.getCmp("promo_name_list").setValue("");
}
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("PromoShareGrid").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("PromoShareGrid").down('#delete').setDisabled(selections.length == 0);
        }
    }
});
Ext.onReady(function () {
    var PromoShareGrid = Ext.create('Ext.grid.Panel', {
        id: 'PromoShareGrid',
        store: PromoShareStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: '活動編號', dataIndex: 'promo_id', width: 100, align: 'center' },
            { header: '活動名稱', dataIndex: 'promo_name', width: 200, align: 'center' },
            { header: '活動描述', dataIndex: 'promo_desc', width: 100, align: 'center' },
            { header: '開始時間', dataIndex: 'promo_start', width: 200, align: 'center' },
            { header: '結束時間', dataIndex: 'promo_end', width: 200, align: 'center' },
            {
                header: '詳細信息', dataIndex: 'this_promo_id', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value) {
                        return "<a href=javascript:onConditionClick(\"" + record.data.promo_id + "\")>" + "詳細信息" + "</a>";
                    }
                    else {
                        return "<font color='red'>" + "詳細信息"+"</font>";
                    }
                }
            },
            {
                header: '狀態', dataIndex: 'promo_active', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 1) {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.promo_id + ")'><img hidValue='0' id='img" + record.data.promo_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                    } else {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.promo_id + ")'><img hidValue='1' id='img" + record.data.promo_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                    }
                }
            }
        ],
        tbar: [
           { xtype: 'button', text: "新增", id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
           { xtype: 'button', text: EDIT, id: 'edit', iconCls: 'icon-user-edit', disabled: true, hidden: true, handler: onEditClick },
           { xtype: 'button', text: "刪除", id: 'delete', iconCls: 'icon-user-edit', disabled: true, hidden: true, handler: onDeleteClick }, '->',
           {
               fieldLabel: "活動名稱", labelWidth: 60, xtype: 'textfield', id: 'promo_name_list',
               listeners: {
                   specialkey: function (field, e) {
                       if (e.getKey() == e.ENTER)
                       {
                           Query();
                       }
                   }

               }
           },
           { xtype: 'combobox', fieldLabel: "狀態", editable:false,labelWidth: 40, id: 'ddlstatus', store: DDLStore, displayField: 'txt', valueField: 'value', value: '2' },
           {
               text: SEARCH,
               iconCls: 'icon-search',
               id: 'btnQuery',
               handler: Query
           },
            {
                text: "重置",
                id: 'czQuery',
                handler: ChongZhiQuery
            }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: PromoShareStore,
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
    Ext.create('Ext.Viewport', {
        layout: 'fit',
        items: [PromoShareGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                PromoShareGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    PromoShareStore.load({ params: { start: 0, limit: 25 } });
});
function dateFormat(value) {
    if (null != value) {
        return Ext.Date.format(new Date(value), 'Y-m-d H:i:s');
    } else {
        return "";
    }
}

/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {
    //addWin.show();
    editFunction(null, PromoShareStore);
}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("PromoShareGrid").getSelectionModel().getSelection();
    //alert(row[0]);
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], PromoShareStore);
    }
}

onDeleteClick = function () {
    var row = Ext.getCmp("PromoShareGrid").getSelectionModel().getSelection();
    if (row.length <= 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format("共" + row.length + "條數據," + "是否確定要刪除嗎?", row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.promo_id + ',';
                }
                Ext.Ajax.request({
                    url: '/PromoShare/DeletePromoShareMessage',//執行方法
                    method: 'post',
                    params: { rid: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            Ext.Msg.alert(INFORMATION, "刪除成功!");
                            PromoShareStore.load();
                        }
                        else {
                            Ext.Msg.alert(INFORMATION, "刪除失敗!");
                            PromoShareStore.load();
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
function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");
    Ext.Ajax.request({
        url: '/PromoShare/GetPromoShareConditionCount',//執行方法
        method: 'post',
        params: { promo_id: id },
        success: function (form, action) {
            var result = Ext.decode(form.responseText);

            if (result.success) {
                if (result.Count > 0) {
                    $.ajax({
                        url: "/PromoShare/UpdateActivePromoShareMaster",
                        data: {
                            "row_id": id,
                            "active": activeValue
                        },
                        type: "POST",
                        dataType: "json",
                        success: function (msg) {
                            PromoShareStore.load();
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
                else {
                    Ext.Msg.alert(INFORMATION, "該活動未設定內容,請先設定活動信息!");
                    PromoShareStore.load();
                }


            }
            else {
                Ext.Msg.alert(INFORMATION, "程序異常!");
                PromoShareStore.load();
            }
        },
        failure: function () {
            Ext.Msg.alert(INFORMATION, FAILURE);
        }
    });
}
onConditionClick = function (promo_id) {
    onConditionClick(promo_id);
}