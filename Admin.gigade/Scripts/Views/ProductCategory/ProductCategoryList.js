var pageSize = 25;
var fatherid = "";
var fathername = "";
/*******************群組管理主頁面****************************/
//群組管理Model
Ext.define('gigade.ProductCategoryModel', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "category_id", type: "int" },
        { name: "category_father_id", type: "int" },
        { name: "category_name", type: "string" },
        { name: "category_sort", type: "int" },
        { name: "category_display", type: "int" },
        { name: "category_show_mode", type: "int" },
        { name: "category_image_in", type: "string" },
        { name: "category_image_out", type: "string" },
        { name: "category_link_mode", type: "int" },
        { name: "category_link_url", type: "string" },
        { name: "banner_image", type: "string" },
        { name: "banner_status", type: "int" },
        { name: "banner_link_mode", type: "int" },
        { name: "banner_link_url", type: "string" },
        { name: "startdate", type: "string" },
        { name: "enddate", type: "string" },
        { name: "category_createdate", type: "string" },
        { name: "category_updatedate", type: "string" },
        { name: "category_ipfrom", type: "string" },
         { name: "category_father_name", type: "string" },
         { name: "status", type: "int" }
    ]
});

var ProductCategoryStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'gigade.ProductCategoryModel',
    proxy: {
        type: 'ajax',
        url: '/ProductCategory/GetProductCategoryList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

//前面選擇框 選擇之後顯示編輯刪除
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("productCategoryGift").down('#edit').setDisabled(selections.length == 0);
            //  Ext.getCmp("productCategoryGift").down('#remove').setDisabled(selections.length == 0);
        }
    }
});


function Query(father_id) {
    fatherid = father_id;
    fathername = ProductCategoryStore.getAt(ProductCategoryStore.find("category_id", father_id)).data.category_name;
    ProductCategoryStore.removeAll();
    Ext.getCmp("productCategoryGift").store.loadPage(1, {
        params: {
            father_id: father_id
        }
    });
}
//加載前先獲取ddl的值
ProductCategoryStore.on('beforeload', function () {
    ProductCategoryStore.remove();
    Ext.apply(ProductCategoryStore.proxy.extraParams, {
        father_id: fatherid == "" ? "" : fatherid
    });
});
//頁面載入
Ext.onReady(function () {
    var productCategoryGift = Ext.create('Ext.grid.Panel', {
        id: 'productCategoryGift',
        store: ProductCategoryStore,
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
            { header: CATEGORYID, dataIndex: 'category_id', width: 70, align: 'center', align: 'center' },
            {
                header: CATEGORYNAME, dataIndex: 'category_name', width: 200, align: 'center'
            },
            { header: FATHERCATEID, dataIndex: 'category_father_id', width: 70, align: 'center', align: 'center', hidden: true },
            {
                header: FATHERCATENAME, dataIndex: 'category_father_name', width: 200, align: 'center'
            },
            {
                header: CATESOET, dataIndex: 'category_sort', width: 70, align: 'center'
            },
            {
                header: CATEBANNER, dataIndex: 'banner_image', width: 200, align: 'center', hidden: true
            },
            {
                header: ISSHOW, dataIndex: 'category_display', width: 70, align: 'center',
                renderer: function (val) {
                    if (val == 1) {
                        return SHOWSTATUS;
                    }
                    else {
                        return "<span style=' color:red'>" + HIDESTATUS + "</span>";
                    }
                }
            },
            {
                header: LINKMODE, dataIndex: 'category_link_mode', width: 100, align: 'center',
                renderer: function (val) {
                    if (val == 2) {
                        return NEWWIN;
                    }
                    else {
                        return OLDWIN;
                    }
                }
            },
             {
                 header: BANNERSTATUS, dataIndex: 'banner_status', width: 100, align: 'center',
                 renderer: function (val) {
                     if (val == 1) {
                         return ACTIVE;
                     }
                     else if (val == 2) {
                         return "<span style=' color:red'>" + NOTACTIVE + "</span>";
                     }
                 }
             },
             {
                 header: BANNERLINKMODE, dataIndex: 'banner_link_mode', width: 100, align: 'center',
                 renderer: function (val) {
                     if (val == 1) {
                         return OLDWIN;
                     }
                     else if (val == 2) {
                         return NEWWIN;
                     }
                 }
             },
              {
                  header: BANNERSTART, dataIndex: 'startdate', width: 100, align: 'center', hidden: true
              },
             {
                 header: BANNEREND, dataIndex: 'enddate', width: 100, align: 'center', hidden: true
             },
             {
                 header: SHOWCHILD,
                 dataIndex: 'category_id',
                 hidden: false,
                 align: 'center',
                 id: 'childCategory',
                 hidden: false,
                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                     return "<a href='javascript:void(0);' onclick='Query(" + value + ")'>" + CLICKSHOWCHILD + "</a>";
                     //var value_name = record.data.category_name;
                     //alert(value + "," + value_name)
                     //Query(value, value_name);
                     //alert("<a href='javascript:void(0);' onclick='" + "Query(" + value + ',"' + value_name + '")' + ">點擊查看子類別</a>")
                     //  return "<a href='javascript:void(0);' onclick='" + "Query(" + value + ',"' + value_name + '")' + ">點擊查看子類別</a>";
                 }
             },
             {
                 header: STATUS,
                 dataIndex: 'status',
                 hidden: false,
                 align: 'center',
                 id: 'status',
                 hidden: false,
                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                     if (value == 1) {
                         fatherid = record.data.category_father_id;
                         fathername = record.data.category_father_name;
                         return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.category_id + "," + record.data.category_father_id + ")'><img hidValue='0' id='img" + record.data.category_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                     } else {
                         return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.category_id + "," + record.data.category_father_id + ")'><img hidValue='1' id='img" + record.data.category_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                     }
                 }
             }
        ],
        tbar: [
            { xtype: 'button', text: ADD, id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
            { xtype: 'button', text: EDIT, id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
            { xtype: 'button', id: 'Export', text: EXPORT, icon: '../../../Content/img/icons/excel.gif', hidden: false, handler: ExportCSV },
            { xtype: 'button', text: PREVSTEP, id: 'back', hidden: false, iconCls: 'icon-search', handler: onBack },
            '->',
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: ProductCategoryStore,
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
        items: [productCategoryGift],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                productCategoryGift.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    ProductCategoryStore.load({ params: { start: 0, limit: 25 } });
});

//添加
onAddClick = function () {
    editFunction(null, ProductCategoryStore, fatherid, fathername);
}
//修改
onEditClick = function () {
    var row = Ext.getCmp("productCategoryGift").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], ProductCategoryStore, null, null);
    }
}
//返回上一頁
onBack = function () {
    if (fatherid != "") {
        $.ajax({
            url: "/ProductCategory/GetFatherId",
            data: {
                "fid": fatherid,
            },
            type: "POST",
            dataType: "json",
            success: function (msg) {
                ProductCategoryStore.remove();
                fatherid = msg.result;
                //ProductCategoryStore.load({ params: { start: 0, father_id: msg.result } });
                Ext.getCmp("productCategoryGift").store.loadPage(1, {
                    params: {
                        father_id: fatherid,
                       // search: Ext.getCmp("search").getValue()
                    }
                });
            },
            error: function (msg) {
                Ext.Msg.alert(INFORMATION, FAILURE);
            }
        });
    }

}


//更改活動狀態(設置活動可用與不可用)
function UpdateActive(id, fatherId) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/ProductCategory/UpdateActive",
        data: {
            "id": id,
            "active": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            ProductCategoryStore.remove();
            if (activeValue == 1) {
                $("#img" + id).attr("hidValue", 0);
                $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
                ProductCategoryStore.load({ params: { father_id: fatherId } });
            } else {
                $("#img" + id).attr("hidValue", 1);
                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                ProductCategoryStore.load({ params: { father_id: fatherId } });
            }
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);
        }
    });
}
//匯出
function ExportCSV() {
    Ext.Ajax.request({
        url: "/ProductCategory/ProductCategoryCSV",
        params: {
            Root: fatherid
        },
        success: function (response) {
            if (response.responseText.split(',')[0] == "true") {
                window.location.href = '../..' + response.responseText.split(',')[2] + response.responseText.split(',')[1];
            }
        }
    });
}