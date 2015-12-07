var pageSize = 25;
var tranAreaId = 0;
//model
Ext.define('gigade.ElementDetailModel', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "element_id", type: "int" },
        { name: "element_type", type: "int" },
        { name: "element_type_name", type: "string" },
        { name: "element_content", type: "string" },
        { name: "product_id", type: "int" },
        { name: "product_name", type: "string" },
        { name: "product_status", type: "int" },
        { name: "element_name", type: "string" },
        { name: "element_link_url", type: "string" },
        { name: "element_link_mode", type: "int" },
        { name: "element_linkmode", type: "string" },
        { name: "element_sort", type: "int" },
        { name: "element_status", type: "int" },
        { name: "element_start", type: "string" },
        { name: "element_end", type: "string" },
        { name: "element_createdate", type: "string" },
        { name: "element_updatedate", type: "string" },
        { name: "create_userid", type: "int" },
        { name: "update_userid", type: "int" },
        { name: 'element_remark', type: 'string' },
        { name: 'packet_id', type: 'int' },
        { name: 'packet_name', type: 'string' },
        { name: 'packet_status', type: 'int' },
        { name: 'category_id', type: 'int' },
        { name: 'category_name', type: 'string' },
        { name: 'kendo_editor', type: 'string' },
        { name: 'element_img_big', type: 'string' }
    ]
});

var ElementDetailStore = Ext.create('Ext.data.Store', {
    autoDestroy: true, //自動消除
    autoLoad: false,
    pageSize: pageSize,
    model: 'gigade.ElementDetailModel',
    proxy: {
        type: 'ajax',
        url: '/Element/GetDetailList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

var ProductStatusStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
                  { "txt": ALLSTATUS, "value": "0" },
                  { "txt": SHANGJIA, "value": "5" },
                  { "txt": XIAJIA, "value": "55" },

    ]
});


//前面選擇框 選擇之後顯示編輯刪除
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdDetail").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("gdDetail").down('#delete').setDisabled(selections.length == 0);
            
        }
    }
});

//var imgsmalltpl = Ext.create('Ext.XTemplate',
//    '<tpl if="' + eletype + "== 1" + '">',
//        '<div style="width:50px;height:50px"><img width="50px" height="50px" src="{element_content}" /><div>',
//     '</tpl>'
//)

ElementDetailStore.on('beforeload', function () {
    Ext.apply(ElementDetailStore.proxy.extraParams,
        {
            packet_id: document.getElementById("packetId").value,
            serchcontent: Ext.getCmp('serchcontent').getValue(),
            search_type: Ext.getCmp('search_type').getValue(),
            product_status: Ext.getCmp('product_status').getValue()

        });
});
function Query(x) {
    if ((Ext.getCmp("search_type").getValue() != null && Ext.getCmp("search_type").getValue() != "") || Ext.getCmp("serchcontent").getValue().trim() != ""
        || Ext.getCmp('serchCate').getValue().trim() != "" || (Ext.getCmp('product_status').getValue() != null && Ext.getCmp('product_status').getValue() != "" || document.getElementById("packetId").value != 0)) {
        ElementDetailStore.removeAll();
        Ext.getCmp("gdDetail").store.loadPage(1, {
            params: {
                searchcontent: Ext.getCmp('serchcontent').getValue(),
                search_type: Ext.getCmp('search_type').getValue(),
                searchCate: Ext.getCmp('serchCate').getValue(),
                product_status: Ext.getCmp('product_status').getValue()
            }
        });
    }
    else {
        Ext.Msg.alert(INFORMATION, SEARCH_LIMIT);
    }
}
//頁面載入
Ext.onReady(function () {
    var gdDetail = Ext.create('Ext.grid.Panel', {
        id: 'gdDetail',
        store: ElementDetailStore,
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
             { header: BANNERID, dataIndex: 'element_id', width: 90, align: 'center' },
             { header: 'packet_id', dataIndex: 'packet_id', width: 100, align: 'center', hidden: true },
             { header: PACKETNAME, dataIndex: 'packet_name', width: 100, align: 'center' },
             {
                 header: BANNERTYPE, dataIndex: 'element_type', width: 100, align: 'center', hidden: true
             },
             { header: BANNERTITLE, dataIndex: 'element_name', width: 150, align: 'center' },
             { header: BANNERTYPE, dataIndex: 'element_type_name', width: 80, align: 'center' },
             {
                 header: IMGSMALL, id: 'imgsmall', colName: 'element_content',
                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                     if (record.data.element_type == 1) {
                         return '<div style="width:50px;height:50px"><a target="_blank", href="' + record.data.element_content + '"><img width="50px" height="50px" src="' + record.data.element_content + '" /></a><div>'
                     } else {
                         return null;
                     }
                 },
                 width: 60, align: 'center', sortable: false, menuDisabled: true
             },

             {
                 header: IMAGE, dataIndex: 'element_content', width: 150, align: 'center',
                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                     if (record.data.element_type == 1) {
                         return value;
                     } else {
                         return null;
                     }
                 }
             },
            {
                header: BANNERCONTENT, dataIndex: 'element_content', width: 150, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.element_type == 2) {
                        return value;
                    } else {
                        return null;
                    }
                }
            },
            {
                header: PROD, dataIndex: 'element_content', width: 150, 
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.element_type == 3) {
                        return record.data.product_id + "  |  " + record.data.product_name;
                    } else {
                        return null;
                    }
                }
            },
            {
                header: "元素圖(大)", id: 'element_img_big', colName: 'element_img_big',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.element_type == 1 || record.data.element_type == 3) {
                        return '<div style="width:50px;height:50px"><a target="_blank", href="' + record.data.element_img_big + '"><img width="50px" height="50px" src="' + record.data.element_img_big + '" /></a><div>'
                    } else {
                        return null;
                    }
                },
                width: 60, align: 'center', sortable: false, menuDisabled: true
            },
            { header: CATEGORYNAME, dataIndex: 'category_name', width: 150, align: 'center' },
            { header: BANNERLINKURL, dataIndex: 'element_link_url', width: 100, align: 'center' },
            { header: BANNERLINKMODE, dataIndex: 'element_link_mode', width: 80, align: 'center', hidden: true },
            { header: BANNERLINKMODE, dataIndex: 'element_linkmode', width: 80, align: 'center' },
            { header: BANNERSORT, dataIndex: 'element_sort', width: 50, align: 'center' },
            { header: BANNERSTATUS, dataIndex: 'element_status', width: 100, align: 'center', hidden: true },
            { header: ESCINFO, dataIndex: 'element_remark', width: 120, align: 'center' },
            { header: BANNERSTSRT, dataIndex: 'element_start', width: 120, align: 'center' },
            { header: BANNEREND, dataIndex: 'element_end', width: 120, align: 'center' },
            { header: BANNERUPDATE, dataIndex: 'element_updatedate', width: 120, align: 'center' },
            {
                header: ISACTIVE,
                dataIndex: 'element_status',
                align: 'center',
                width: 60,
                id: 'controlstatus',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 1) {
                        return "<a href='javascript:void(0);' onclick='UpdateStatus(" + record.data.element_id + "," + record.data.packet_id + ")'><img hidValue='0' id='img" + record.data.element_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                    } else {
                        return "<a href='javascript:void(0);' onclick='UpdateStatus(" + record.data.element_id + "," + record.data.packet_id + ")'><img hidValue='1' id='img" + record.data.element_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                    }
                }
            }
        ],
        tbar: [
            { xtype: 'button', text: ADD, id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
            { xtype: 'button', text: EDIT, id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
            {
                xtype: 'button', text: "刪除", id: 'delete', iconCls: 'icon-user-edit', disabled: true, handler: onDeleteClick,
                listeners: {
                    'beforerender': function () {
                        //alert(document.getElementById("packetId").value);
                        if (document.getElementById("packetId").value ==0) {
                            Ext.getCmp('delete').hide(true);
                        }
                        //else {
                        //}
                    }
                }

            },
            '->',
            {
                xtype: 'combobox',
                fieldLabel: PRODSTATUS,
                id: 'product_status',
                name: 'product_status',
                labelWidth: 60,
                width: 180,
                editable: false,
                typeAhead: true,
                queryModel: 'local',
                forceSelection: false,
                store: ProductStatusStore,
                displayField: 'txt',
                valueField: 'value',
                emptyText: SELECT,
                listeners: {
                    change: function (newValue, oldValue, e) {
                        if (newValue) {
                            if (Ext.getCmp("product_status").getValue() != "") {
                                Query(1);
                            }
                        }
                    }
                }
            },
             {
                 xtype: 'combobox',
                 fieldLabel: ELEMENTTYPE,
                 id: 'search_type',
                 name: 'search_type',
                 labelWidth: 60,
                 width: 180,
                 margin: '0 10 0 0',
                 editable: false,
                 typeAhead: true,
                 queryModel: 'remote',
                 forceSelection: false,
                 store: elementTypeStore,
                 displayField: 'parameterName',
                 valueField: 'parameterCode',
                 emptyText: SELECT,
                 listeners: {
                     change: function (newValue, oldValue, e) {
                         if (newValue) {
                             if (Ext.getCmp("search_type").getValue() != "") {
                                 Query(1);
                             }
                         }
                     }
                 }
             },
              {
                  xtype: 'textfield', fieldLabel: KEY, id: 'serchcontent', labelWidth: 45, width: 165, listeners: {
                      specialkey: function (field, e) {
                          if (e.getKey() == e.ENTER) {
                              Query(1);
                          }
                      }
                  }
              },
                  {
                      xtype: 'textfield', fieldLabel: SEARCHCATE, id: 'serchCate', labelWidth: 80, width: 200, listeners: {
                          specialkey: function (field, e) {
                              if (e.getKey() == e.ENTER) {
                                  Query(1);
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
                 text: RESET,
                 id: 'btn_reset',
                 listeners: {
                     click: function () {
                         Ext.getCmp("serchcontent").setValue("");
                         Ext.getCmp("search_type").setValue("");
                         Ext.getCmp("serchCate").setValue("");
                         Ext.getCmp("product_status").setValue("");
                     }
                 }
             }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: ElementDetailStore,
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
        viewConfig: {
            forceFit: true, getRowClass: function (record, rowIndex, rowParams, store) {
                //alert(record.data.type);
                if (record.data.product_status != 5 && record.data.element_type == 3) {
                    // alert(record.data.type);
                    return 'product_status_xia';
                }
            }
        },
        selModel: sm
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [gdDetail],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdDetail.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    if (document.getElementById("packetId").value != 0) {
        ElementDetailStore.load({ params: { start: 0, limit: 25 } });
    }

});

//添加
onAddClick = function () {
    if (document.getElementById("packetId").value != 0) {
        Ext.Ajax.request({
            url: '/Element/GetId',
            method: 'post',
            async: true,
            params: {
                PacketId: document.getElementById("packetId").value
            },
            success: function (form, action) {
                var result = Ext.decode(form.responseText);
                if (result.success) {
                    editFunction(null, ElementDetailStore, null);
                }
                else {
                    Ext.Msg.alert(INFORMATION, PACKETNUMBER);
                }
            }
        });
    } else {
        editFunction(null, ElementDetailStore, null);
    }


}
//修改
onEditClick = function () {
    var row = Ext.getCmp("gdDetail").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        if (row[0].data.packet_status == 0) {
            Ext.Msg.alert(INFORMATION, EDITTIP);
        }
        else {
            editFunction(row[0], ElementDetailStore, null);
        }

    }
}

onDeleteClick = function () {
    var row = Ext.getCmp("gdDetail").getSelectionModel().getSelection();
    var rowIDs="";
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length >= 1) {
        for (var i = 0; i < row.length; i++) {
            rowIDs += row[i].data.element_id+"|";
        }
        Ext.Msg.confirm("確認", Ext.String.format("刪除選中" + row.length + "條數據?", row.length), function (btn) {
            if (btn == 'yes') {
                var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
                myMask.show();
                Ext.Ajax.request({
                    url: '/Element/DeleteElementDetail',
                    params: {
                        rowIDs: rowIDs,
                    },
                    success: function (form, action) {
                        myMask.hide();
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            Ext.Msg.alert("提示信息", "刪除成功！");
                            ElementDetailStore.load();
                        }
                        else {
                            Ext.Msg.alert("提示信息", "刪除失敗！");
                            
                        }
                    },
                    failure: function () {
                        myMask.hide();
                        Ext.Msg.alert("提示信息", "出現異常！");
                       
                    }
                });
            }
        });

    }
}

//更改狀態
function UpdateStatus(id, packet_id) {
    var statusValue = $("#img" + id).attr("hidValue");
    var limitN = 0;
    var listN = 0;
    var boolFull = false;
    Ext.Ajax.request({
        url: "/Element/GetId",
        method: 'post',
        async: false, //true為異步，false為同步
        params: {
            PacketId: packet_id
        },
        success: function (form, action) {
            var result = Ext.decode(form.responseText);
            if (result.success) {
                boolFull = false;

            } else {
                boolFull = true;
            }
        }
    });
    if (boolFull == false || (boolFull == true && statusValue == "0")) {
        $.ajax({
            url: "/Element/UpdateDetailStatus",
            data: {
                "id": id,
                "status": statusValue
            },
            type: "POST",
            dataType: "json",
            success: function (msg) {
                Query();
                if (statusValue == 1) {
                    $("#img" + id).attr("hidValue", 0);
                    $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
                } else {
                    $("#img" + id).attr("hidValue", 1);
                    $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                }
            },
            error: function (msg) {
                Ext.Msg.alert(INFORMATION, EDITERROR);
            }
        });
    } else {
        Ext.Msg.alert(INFORMATION, NUMBERLIMITTIP);
    }
}


