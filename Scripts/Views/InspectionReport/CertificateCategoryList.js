
var pageSize = 25;

//證書Model
Ext.define('gigade.CertificateCategory', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "frowID", type: "int" },
        { name: "rowID", type: "int" },
        { name: "certificate_categoryname", type: "string" },
        { name: "certificate_categorycode", type: "string" },
        { name: "status", type: "bool" },
        { name: "k_user", type: "int" },
        { name: "k_date", type: "string" },
        { name: "k_user_tostring", type: "string" },
         { name: "frowID", type: "int" },
        { name: "certificate_category_childname", type: "string" },
        { name: "certificate_category_childcode", type: "string" }
    ]
});

//
//證書Store
var CertificateCategoryStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.CertificateCategory',
    proxy: {
        type: 'ajax',
        url: '/InspectionReport/GetCertificateCategory',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
var rowId;
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("CertificateCategoryGrid").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("CertificateCategoryGrid").down('#delete').setDisabled(selections.length == 0);
        }
    }
});
CertificateCategoryStore.on('beforeload', function () {
    Ext.apply(CertificateCategoryStore.proxy.extraParams, {
        searchcontent: Ext.getCmp("searchcon").getValue(),//搜索內容--
        //start_time: Ext.getCmp("start_time").getValue(),//開始時間--start_time--created
        //end_time: Ext.getCmp("end_time").getValue(),//結束時間--end_time--created
        //success: Ext.getCmp("successquery").getValue(),//發送狀態--success
        //SmsId: document.getElementById("SMSID").value
    })

});

function Query(x) {
    if (Ext.getCmp("searchcon").getValue().trim() != "") {
        CertificateCategoryStore.removeAll();
        Ext.getCmp("CertificateCategoryGrid").store.loadPage(1, {
            params: {
                searchcontent: Ext.getCmp("searchcon").getValue(),//搜索內容--
                //start_time: Ext.getCmp("start_time").getValue(),//開始時間--start_time--created
                //end_time: Ext.getCmp("end_time").getValue(),//結束時間--end_time--created
                //success: Ext.getCmp("successquery").getValue(),//發送狀態--success
                //SmsId: document.getElementById("SMSID").value
            }
        });
    }
    else {
        Ext.Msg.alert(INFORMATION, "請輸入搜索內容");
    }
}

Ext.onReady(function () {

    var CertificateCategoryGrid = Ext.create('Ext.grid.Panel', {
        id: 'CertificateCategoryGrid',
        store: CertificateCategoryStore,
        flex: 8.8,
        // flex: 10,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "ID", dataIndex: 'rowID', width: 160, align: 'center' },
            {
                header: '證書-大類名稱', dataIndex: 'certificate_categoryname', width: 90, align: 'center'
            },
            { header: "證書-大類CODE", dataIndex: 'certificate_categorycode', width: 250, align: 'center' },
            {
                header: "證書-小類名稱", dataIndex: 'certificate_category_childname', width: 250, align: 'center'
            },
              { header: "證書-小類CODE", dataIndex: 'certificate_category_childcode', width: 160, align: 'center' },
              { header: "創建人", dataIndex: 'k_user_tostring', width: 160, align: 'center' },
              { header: "創建時間", dataIndex: 'k_date', width: 160, align: 'center' },
            //{
            //    header: "狀態", dataIndex: 'status', width: 100, hidden:true, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
            //        if (value == 1) {
            //            return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.rowID + ")'><img hidValue='0' id='img" + record.data.rowID + "' src='../../../Content/img/icons/accept.gif'/></a>";
            //        } else {
            //            return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.rowID + ")'><img hidValue='1' id='img" + record.data.rowID + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
            //        }
            //    }
            //}
          
        ],
        tbar: [
             { xtype: 'button', text: "新增", id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },//
           { xtype: 'button', text: "編輯", id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },//
           { xtype: 'button', text: "刪除", id: 'delete', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onDeleteClick },
           "->",
            {
                xtype: 'textfield',
                id: 'searchcon',
                margin: '0 5px',
                name: 'searchcon',
                fieldLabel: '證書大/小類名稱/CODE',
                labelWidth: 135,
                listeners: {
                    specialkey: function (field, e) {
                        if (e.getKey() == Ext.EventObject.ENTER) {
                            Query();
                        }
                    }
                }
            },
             {
                text: SEARCH,
                iconCls: 'icon-search',
                id: 'btnQuery',
                handler: Query
             },
             {
                 xtype: 'displayfield',
                 id: 'isdelete',
                 name: 'isdelete',

             }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: CertificateCategoryStore,
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
        selModel:sm
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [ CertificateCategoryGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                CertificateCategoryGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            },
        }
    });
    
    ToolAuthority();
});

//*********新增********//
onAddClick = function () {
    editFunction(null, CertificateCategoryStore);
}

//*********編輯********//
onEditClick = function () {
    var row = Ext.getCmp("CertificateCategoryGrid").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, SELECTONE);
    } else {
        editFunction(row[0], CertificateCategoryStore);
    }
}



//初始時間
function Tomorrow(days) {
    var d;
    var s = "";
    d = new Date();                             // 创建 Date 对象。
    s += d.getFullYear() + "/";                     // 获取年份。
    s += (d.getMonth() + 1) + "/";              // 获取月份。
    s += d.getDate() + days;                          // 获取日。
    return (new Date(s));                                 // 返回日期。
}

//更改活動狀態(設置活動可用與不可用)
function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/InspectionReport/UpdateActive",
        data: {
            "id": id,
            "active": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            CertificateCategoryStore.load();
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

onDeleteClick = function () {
    var row = Ext.getCmp("CertificateCategoryGrid").getSelectionModel().getSelection();
    if (row.length <= 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else {
            Ext.Msg.confirm(CONFIRM, Ext.String.format("刪除選中" + row.length + "條數據?", row.length), function (btn) {
                if (btn == 'yes') {
                    var rowIDs = '';
                    var frowIDs = '';
                    var ids = '';
                    for (var i = 0; i < row.length; i++) {
                        rowIDs += row[i].data.rowID + ',';
                        frowIDs += row[i].data.frowID + ',';
                        ids += row[i].data.rowID + ',' + row[i].data.frowID + "|";
                    }
                    Ext.Ajax.request({
                        url: '/InspectionReport/DeleteCertificateCategory',//執行方法
                        method: 'post',
                        params: {
                            rid: rowIDs,
                            frid: frowIDs,
                            ids: ids,
                        },
                        success: function (form, action) {
                            var result = Ext.decode(form.responseText);
                            if (result.success) {
                                if (result.msg == 1) {
                                    Ext.Msg.alert(INFORMATION, "刪除成功!");
                                    CertificateCategoryStore.loadPage(1);
                                }
                               else if (result.msg == -1) {
                                   Ext.Msg.alert(INFORMATION, "有數據已被使用,無法刪除！");
                                }
                                else {
                                    Ext.Msg.alert(INFORMATION, "刪除失敗!");
                                    CertificateCategoryStore.loadPage(1);
                                }
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, "操作失敗!");
                                CertificateCategoryStore.loadPage(1);
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
