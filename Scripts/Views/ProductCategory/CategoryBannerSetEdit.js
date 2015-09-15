editFunction = function (row, store) {

    var bannerCateFatherStore = Ext.create("Ext.data.Store", {
        model: 'gigade.bannerCate',
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: '/Parameter/QueryPara?paraType=banner_cate',
            noCache: false,
            getMethod: function () { return 'get'; },
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'items'
            }
        }
    });
    //群組管理Model
    Ext.define('gigade.ProductCategoryBannerModel', {
        extend: 'Ext.data.Model',
        fields: [
            { name: "category_id", type: "int" },
            { name: "category_name", type: "string" },
            { name: "category_father_id", type: "int" },
            { name: "category_sort", type: "int" },
            { name: "category_display", type: "int" },
            { name: "category_link_mode", type: "int" },
            { name: "status", type: "int" }
        ]
    });
    var bannercateSetEdit_store = Ext.create('Ext.data.Store', {
        model: 'gigade.ProductCategoryBannerModel',
        proxy: {
            type: 'ajax',
            url: '/ProductCategory/GetProductByCategorySet',
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'item'
            }
        }
    });
    bannercateSetEdit_store.on("beforeload", function () {
        Ext.apply(bannercateSetEdit_store.proxy.extraParams, {
            banner_cateid: Ext.getCmp('category_father_id').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('category_father_id').getValue())
        });
    });

    //顯示類別中商品的grid
    var bannercateSetEditGrid = new Ext.grid.Panel({
        id: 'bannercateSetEditGrid',
        store: bannercateSetEdit_store,
        autoScroll: true,
        region: 'center',
        height: 320,
        columnLines: true,
        // frame: true,
        columns: [

                  { header: CATEGORYID, dataIndex: 'category_id', width: 60, align: 'center' },
            {
                header: CATEGORYNAME, dataIndex: 'category_name', width: 100, align: 'center'
            },
            { header: FATHERCATEID, dataIndex: 'category_father_id', width: 60, align: 'center' },
            {
                header: CATESOET, dataIndex: 'category_sort', width: 60, align: 'center'
            },
                {
                    header: ISSHOW, dataIndex: 'category_display', width: 60, align: 'center',
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
                    else if (val == 1) {
                        return OLDWIN;
                    }
                }
            }
        ],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        }
    });

    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/ProductCategory/CategoryBannerSetSave',
        items: [
                    {
                        xtype: 'textfield',
                        fieldLabel: 'RowID',
                        id: 'row_id',
                        name: 'row_id',
                        submitValue: true,
                        hidden: true,
                        width: 300
                    },
                    {
                        xtype: 'combobox',
                        fieldLabel: BANNERCATID,
                        id: 'category_father_id',
                        name: 'category_father_id',
                        allowBlank: false,
                        editable: false,
                        typeAhead: true,
                        forceSelection: false,
                        queryModel: 'local',
                        store: bannerCateFatherStore,
                        displayField: 'parameterName',
                        valueField: 'parameterCode',
                        emptyText: SELECT,
                        listeners: {
                            select: function (combo, records, eOpts) {
                                bannercateSetEdit_store.removeAll();
                                bannercateSetEdit_store.load({
                                    params: { banner_cateid: Ext.getCmp('category_father_id').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('category_father_id').getValue()) }
                                ,
                                    callback: function () {
                                        if (bannercateSetEdit_store.data.items.length == 0) {
                                            Ext.Msg.alert(INFORMATION, BANNERCATIP);
                                        }
                                    }
                                });
                            }
                        }
                    },
                    bannercateSetEditGrid
        ],
        buttons: [{
            formBind: true,
            disabled: true,
            text: SAVE,
            handler: function () {
                var form = this.up('form').getForm();
                var insertValues = "";
                var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "保存中..." });
                myMask.show();
                if (form.isValid()) {
                    var categoryData = Ext.getCmp("bannercateSetEditGrid").store.data.items;
                    if (categoryData.length != 0) {
                        for (var i = 0; i < categoryData.length; i++) {
                            var category_id = categoryData[i].get("category_id");
                            var category_name = categoryData[i].get("category_name");
                            var category_father_id = categoryData[i].get("category_father_id");
                            //  var category_father_name = categoryData[i].get("category_father_name");
                            var category_sort = categoryData[i].get("category_sort");
                            var category_display = categoryData[i].get("category_display");
                            var category_link_mode = categoryData[i].get("category_link_mode");
                            var status = categoryData[i].get("status");
                            insertValues += category_id + "," + category_name + "," + category_father_id + "," + category_sort + "," + category_display + "," + category_link_mode + "," + status + ";";
                        }
                    }

                    form.submit({
                        params: {
                            insertValues: insertValues,
                            banner_cateid: Ext.getCmp('category_father_id').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('category_father_id').getValue())
                        },
                        timeout: 60000,
                        success: function (form, action) {
                            myMask.hide();
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, SAVESUCCESS);
                                //treeStore.load();
                                editWin.close();
                                store.load();

                            }
                            else {
                                Ext.Msg.alert(INFORMATION, SAVEFILURE);
                                editWin.close();
                            }
                        },
                        failure: function (form, action) {
                            myMask.hide();
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert(INFORMATION, SAVEFILURE);
                            editWin.close();
                        }
                    });
                }
            }
        }]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: ADDCATEGORY,
        iconCls: 'icon-user-edit',
        id: 'editWin',
        width: 500,
        // height: 410,
        y: 100,
        layout: 'fit',
        items: [editFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: ISCLOSE,
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('editWin').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }
        ]
    });
    editWin.show();
}
