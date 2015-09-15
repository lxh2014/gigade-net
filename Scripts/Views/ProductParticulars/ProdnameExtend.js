/// <reference path="../../Ext4.0/ext-all.js" />
//var myDate = new Date();
//var year = myDate.getFullYear();
//var month = myDate.getMonth();
//var day = myDate.getDate();
//var hour = myDate.getHours();


Ext.define('GIGADE.SITE', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'Site_Id', type: 'int' },
        { name: 'Site_Name', type: 'string' }
    ]
});

var siteStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.SITE',
    proxy: {
        type: 'ajax',
        url: '/Product/GetSite',
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    },
    listeners: {
        load: function () {
            siteStore.insert(0, { Site_Id: '0', Site_Name: VALUE_ALL });
            //edit by zhuoqin0830w  2015/03/18   註釋站臺初始值
            //Ext.getCmp("pro_site").setValue(0);
        }
    },
    autoLoad: true
});

var siteStore2 = Ext.create('Ext.data.Store', {
    model: 'GIGADE.SITE',
    proxy: {
        type: 'ajax',
        url: '/Product/GetSite',
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    },
    listeners: {
        load: function () {
            siteStore2.insert(0, { Site_Id: '0', Site_Name: VALUE_ALL });
            //Ext.getCmp("search_site").setValue(0);
        }
    },
    autoLoad: true
});

//創建model和store
Ext.define('gigade.prodnamextend', {
    extend: 'Ext.data.Model',
    idProperty: 'Key_Id',
    fields: [
        { name: 'Rid', type: 'int' },
        { name: 'Product_Id', type: 'int' },
        { name: 'Price_Master_Id', type: 'int' },
        { name: 'Item_Id', type: 'int' },
        { name: 'Site_Name', type: 'string' },
        { name: 'User_Level', type: 'string' },
        { name: 'User_Id', type: 'string' },
        { name: 'Site_Id', type: 'int' },
        { name: 'Product_Prefix', type: 'string' },
        { name: 'Product_Name', type: 'string' },
        { name: 'Product_Sz', type: 'string' },
        { name: 'Product_Suffix', type: 'string' },
        { name: 'Event_Start', type: 'string' },
        { name: 'Event_End', type: 'string' },
        { name: 'Kuser', type: 'string' },
        { name: 'Level_Name', type: 'string' },
        { name: 'Kdate', type: 'string' },
        { name: 'Flag', type: 'int' },
        { name: 'Key_Id', type: 'int' } //edit by wwei0216w 2014/12/30
    ]
});

var proname_store = Ext.create('Ext.data.Store', {
    model: 'gigade.prodnamextend',
    proxy: {
        type: 'ajax',
        url: '/ProductParticulars/QueryProdname',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'item'
        }
    }
});

Ext.apply(Ext.form.field.VTypes, {
    //日期筛选
    daterange: function (val, field) {
        var date = field.parseDate(val);
        if (!date) {
            return false;
        }
        if (field.startDateField && (!this.dateRangeMax || (date.getTime() != this.dateRangeMax.getTime()))) {
            var start = Ext.getCmp(field.startDateField);
            start.setMaxValue(date);
            start.validate();
            this.dateRangeMax = date;
        }
        else if (field.endDateField && (!this.dateRangeMin || (date.getTime() != this.dateRangeMin.getTime()))) {
            var end = Ext.getCmp(field.endDateField);
            end.setMinValue(date);
            end.validate();
            this.dateRangeMin = date;
        }
        return true;
    },
    daterangeText: START_BEFORE_END
});

Ext.onReady(function () {
    var save_button = {
        id: 'eventUpdate',
        width: 100,
        margin: '0 0 0 10',
        border: '10 5 3 10',
        xtype: 'button',
        iconCls: 'ui-icon ui-icon-checked',
        text: SAVE,
        listeners: {
            click: Save
        }
    };

    var dockedItem = [{
        id: 'dockedItem',
        xtype: 'toolbar',
        layout: 'column',
        dock: 'top',
        items: [{
            xtype: 'combobox',
            fieldLabel: SITE,
            labelWidth: 40,
            editable: false,
            allowBlank: false,
            store: siteStore,
            queryMode: 'local',
            displayField: 'Site_Name',
            valueField: 'Site_Id',
            id: 'pro_site',
            name: 'pro_site'
        }, {
            xtype: 'textfield',
            fieldLabel: PREFIX,
            id: 'product_prefix',
            name: 'Product_Prefix',
            labelWidth: 40
        }, {
            xtype: 'textfield',
            fieldLabel: SUFFIX,
            id: 'product_suffix',
            name: 'Product_Suffix',
            labelWidth: 40
        }, {
            xtype: 'datefield',
            format: 'Y/m/d',
            fieldLabel: START_TIME,
            id: 'start_from_date',
            name: 'start_from_date',
            labelWidth: 60,
            hidden: true,
            vtype: 'daterange',
            editable: false,
            minValue: new Date(),
            value: new Date(),
            allowBlank: false,
            endDateField: 'start_to_date'
        },
        {
            xtype: 'datefield',
            format: 'Y/m/d',
            vtype: 'daterange',
            fieldLabel: END_TIME,
            id: 'start_to_date',
            name: 'start_to_date',
            labelWidth: 60,
            editable: false,
            celleditable: false,
            allowBlank: false,
            minValue: new Date(),
            startDateField: 'start_from_date'
        }, {
            xtype: 'button',
            text: UPDATE,
            id: 'btn_modify',
            handler: function () { setValue(1); },
            margin: '0 0 0 10',
            iconCls: 'ui-icon ui-icon-pencil'
        }, {
            text: RESET,
            id: 'btn_reset',
            margin: '0 0 0 10',
            iconCls: 'ui-icon ui-icon-reset',
            listeners: {
                click: function () {
                    //edit by zhuoqin0830w  2015/03/18   註釋站臺初始值
                    //Ext.getCmp("pro_site").setValue(siteStore.data.items[0].data.Site_Id);
                    Ext.getCmp("pro_site").reset();
                    Ext.getCmp("product_prefix").setValue("");
                    Ext.getCmp("product_suffix").setValue("");
                    Ext.getCmp("start_from_date").setValue("");
                    Ext.getCmp("start_to_date").setValue("");
                }
            }
        }]
    }, {
        xtype: 'toolbar',
        layout: 'column',
        dock: 'bottom',
        items: [save_button]
    }];

    var tools = [{
        xtype: 'textfield',
        fieldLabel: PRODUCT_ID,
        allowBlank: false,
        labelWidth: 60,
        width: 392,
        id: 'product_id',
        name: 'product_id',
        regex: /^[0-9,]+$/,
        enableKeyEvents: true,
        listeners: {
            keyup: function (e, event) {
                if (event.keyCode == 13) {
                    Search();
                }
            }
        }
    }, {
        xtype: 'combobox',
        fieldLabel: SITE,
        labelWidth: 40,
        editable: false,
        allowBlank: false,
        store: siteStore2,
        queryMode: 'local',
        displayField: 'Site_Name',
        valueField: 'Site_Id',
        id: 'search_site',
        name: 'search_site'
    }, {
        xtype: 'checkbox',
        margin: '0 0 0 20',
        //fieldLabel: 過期,
        boxLabel: STALE,
        id: 'isOverdue'
    }, {
        xtype: 'button',
        text: SEARCH,
        id: 'btn_search',
        handler: Search,
        width: 60,
        margin: '0 0 0 10',
        iconCls: 'ui-icon ui-icon-search-2'
    }, '->', {
        xtype: 'fieldcontainer',
        //margin: '0 0 0 180',
        layout: 'column',
        items: [{
            xtype: 'displayfield',
            value: '◤' + PARTICULAR_PRODUCT//特殊商品
        }, {
            xtype: 'displayfield',
            value: '<span style="background-color:#90EE90">' + USING_PRODUCT + '</span>'//作用中商品(可修改結束日期)
        }, {
            xtype: 'splitter',
            width: 10
        }, {
            xtype: 'displayfield',
            value: '<span style="background-color:#87CEEB">' + INSPECT_PRODUCT + '</span> '//審核中商品(可修改結束日期)
        }, {
            xtype: 'splitter',
            width: 10
        }, {
            xtype: 'displayfield',
            value: '<span style="background-color:#F08080">' + STALE_PRODUCT + '</span>◢'//過期商品(不可修改)
        }]
    }];

    var cellEditing = Ext.create('Ext.grid.plugin.CellEditing', {
        clicksToEdit: 1,
        listeners: {
            beforeedit: function (edit, e, record) {
                if (edit.record.data["Flag"] == '3') {
                    return false;
                } else if (edit.record.data["Flag"] == '2' || edit.record.data["Flag"] == '1') {
                    if (edit.field != "Event_End") {
                        return false;
                    }
                }
            },
            edit: function (edit, e) {
                var val = e.value;
                if (e.field == "Event_End" || e.field == "Event_Start") {
                    if (Ext.Date.format(new Date(val * 1000), 'Y/m/d') == '1970/01/01')
                        proname_store.getAt(e.rowIdx).set(e.column.dataIndex, "");
                    if (new Date(val).toString() != "Invalid Date") {
                        proname_store.getAt(e.rowIdx).set(e.column.dataIndex, new Date(val).getTime() / 1000);
                    }
                }
            }
        }
    });

    var proGrid = Ext.create('Ext.grid.Panel', {
        id: 'proGrid',
        store: proname_store,
        border: '1px',
        columnLines: true,
        tbar: tools,
        dockedItems: dockedItem,
        plugins: [cellEditing],
        viewConfig: {
            getRowClass: function (record, rowIndex, rowParams, store) {  //指定行的样式
                if (record.data["Flag"] == 1) {
                    return 'grid_blue';
                } else if (record.data["Flag"] == 2) {
                    return 'grid-green';
                } else if (record.data["Flag"] == 3) {
                    return 'grid-red';
                }
            }
        },
        columns: [
            { header: SEQUENCE_NUMBER, xtype: 'rownumberer', width: 46, align: 'center' },
            { header: PRODUCT_ID, dataIndex: 'Product_Id', align: 'center', width: 100, menuDisabled: true, sortable: false },
            { header: SITE, dataIndex: 'Site_Name', width: 100, align: 'center', menuDisabled: true, sortable: false },
            { header: SITE_ID, dataIndex: 'Site_Id', width: 100, align: 'center', menuDisabled: true, sortable: false, hidden: true },
            { header: 'KeyId', dataIndex: 'Key_Id', width: 100, align: 'center', menuDisabled: true, sortable: false, hidden: true },
            { header: USER_LEVEL, dataIndex: 'Level_Name', width: 100, align: 'center', menuDisabled: true, sortable: false },
            {
                header: USER_ID, dataIndex: 'User_Id', width: 100, align: 'center', menuDisabled: true, renderer: function (val) {
                    if (val == 0) {
                        return "";
                    } else {
                        return val;
                    }
                }, sortable: false
            },
            { header: 'PriceMastId', dataIndex: 'Price_Master_Id', width: 100, align: 'center', menuDisabled: true, sortable: false, hidden: true },
            { header: STATUS, dataIndex: 'Flag', width: 100, align: 'center', menuDisabled: true, sortable: false, hidden: true },
            {
                header: PREFIX, dataIndex: 'Product_Prefix', width: 150, align: 'right', menuDisabled: true, renderer: function (val) {
                    return val == "" ? "" : '〖' + val + '〗';
                }, sortable: false, editor: { xtype: 'textfield' }
            },
            {
                header: PRODUCT_NAME, dataIndex: 'Product_Name', width: 200, align: 'center', menuDisabled: true, sortable: false,
                renderer: function (value, cellmeta, record) {  //edit by wwei 2014/12/17
                    return value + (record.data.prod_sz ? ' (' + record.data.prod_sz + ')' : '');
                }
            },
            {
                header: SUFFIX, dataIndex: 'Product_Suffix', width: 150, align: 'left', menuDisabled: true, renderer: function (val, metaData, record, rowIndex, colIndex) {
                    return val == "" ? "" : '〖' + val + '〗';
                }, sortable: false, editor: { xtype: 'textfield' }
            }, {
                header: BEGIN_DATE, dataIndex: 'Event_Start', width: 140, align: 'center', sortable: false, menuDisabled: true, hidden: true,//开始時間
                renderer: function (val) {
                    if (Ext.Date.format(new Date(val * 1000), 'Y/m/d') == '1970/01/01')
                        return '';
                    if (val instanceof Date) {
                        return Ext.Date.format(new Date(val), 'Y/m/d');
                    } else {
                        return Ext.Date.format(new Date(val * 1000), 'Y/m/d');
                    }
                }, field: {
                    xtype: 'datefield',
                    allowBlank: false, format: 'Y/m/d',
                    minValue: new Date()
                }
            },
            {
                header: END_DATE, dataIndex: 'Event_End', width: 140, align: 'center', sortable: false, menuDisabled: true,//結束時間
                renderer: function (val) {
                    if (Ext.Date.format(new Date(val * 1000), 'Y/m/d') == '1970/01/01')
                        return '';
                    if (val instanceof Date) {
                        return Ext.Date.format(new Date(val), 'Y/m/d');
                    } else {
                        return Ext.Date.format(new Date(val * 1000), 'Y/m/d');
                    }
                }, field: {
                    xtype: 'datefield',
                    allowBlank: false, format: 'Y/m/d',
                    minValue: new Date()
                }
            }
        ],
        listeners: {
            viewready: function () {
                setShow(proGrid, 'colName');
            },
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            },
            beforeselect: function (e, recode, index, obj) {
                switch (recode.data["Flag"]) {
                    case 0:
                    case 1:
                        break;
                    case 2:
                    case 3:
                    case 4:
                        return false;
                        break;
                }
            }
        }
    });

    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        border: true,
        items: [proGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                proGrid.height = document.documentElement.clientHeight <= 700 ? document.documentElement.clientHeight - 5 : document.documentElement.clientHeight;
                this.doLayout();
            }
        }
    });
});



function Search() {
    Ext.getCmp('product_id').setValue(Ext.getCmp('product_id').getValue().replace(/\s+/g, ','));

    var search_site = Ext.getCmp("search_site");
    if (!Ext.getCmp('product_id').isValid()) return;
    if (!search_site.isValid()) {
        Ext.Msg.alert(MESSAGE, PLEASE_SELECT_SITE);//提示,請選擇站臺
        return;
    }
    proname_store.removeAll();
    proname_store.load({  //edit by wwei0216w 沒有分頁不是用loadPage
        params: {
            Site_Id: Ext.getCmp("search_site").getValue(),
            Ids: Ext.getCmp('product_id').getValue(),
            isOverdue: Ext.getCmp('isOverdue').getValue()
        },
        callback: function () { //查詢的時候將前後綴賦予查詢出的商品
            setValue(2);//查詢設值
        }
    });
}

function Save() {
    if (Ext.getCmp('isOverdue').getValue()) return;
    var myMask = new Ext.LoadMask(Ext.getBody(), {
        msg: 'Loading...'
    });
    myMask.show();
    if (proname_store.getCount() < 1) {
        myMask.hide();
        return;
    }
    var upDataStore = proname_store.getUpdatedRecords(); //獲得修改過的store
    if (!upDataStore.length) {
        myMask.hide();
        Ext.Msg.alert(INFORMATION, NON_DATA_EDIT);//沒有數據被修改
        return;
    }
    var site_id = Ext.getCmp('pro_site').getValue();
    if (site_id == null) {
        myMask.hide();
        Ext.Msg.alert(TITLEMESSAGE, PLEASE_WRITE_MUST_OPTION);//消息提示,請填寫必要選項
        return;
    }

    var prodName = new Array();
    for (var i = 0, j = upDataStore.length ; i < j; i++) {
        var record = upDataStore[i];
        if (record.get("Site_Id") == site_id || site_id == 0) {
            if (record.get("Event_End") == "0") {
                Ext.Msg.alert(INFORMATION, VERIFY_TIME);
                myMask.hide();
                return;
            }
            if (record.get("Product_Prefix") == "" && record.get("Product_Suffix") == "") {
                Ext.Msg.alert(INFORMATION, SUFFIX_MUST_WRITE_ONE);
                myMask.hide();
                return;
            }

            prodName.push({
                "Rid": record.get("Rid"),
                "Product_Id": record.get("Product_Id"),
                "Flag": record.get("Flag"), //作用中的商品可以修改時間,根據Flag來判斷商品是保存還是延後前後綴時間 edit by wwei 2014/12/18
                "Item_Id": record.get("Item_Id"),
                "Price_Master_Id": record.get("Price_Master_Id"),
                "Product_Name": record.get("Product_Name"),
                "Product_Prefix": record.get("Product_Prefix"),
                "Product_Suffix": record.get("Product_Suffix"),
                "Event_Start": record.get("Event_Start"),
                "Event_End": record.get("Event_End"),
                "Kuser": record.get("Kuser")
            });
        }
    }
    var prodNameExtend = JSON.stringify(prodName);
    //add by wwei0215/8/27
    /*
    存在一種情況
    列表中的站臺里並沒有要修改的站臺的信息
    eg:
    要修改中信專區站臺的前後綴
    該商品並沒有中信專區的相關信息

    通過下面判斷
    要更新的數據沒有,但做出了列表修改
    可以預防該情況
    */
    if (prodNameExtend == '[]' && upDataStore.length != 0)
    {
        Ext.Msg.alert(INFORMATION, NOMESSAGEINFOOFSITE);
        myMask.hide();
        return;
    }
    Ext.Ajax.request({
        url: '/ProductParticulars/SaveProdname',
        params: {
            prodNameExtend: prodNameExtend
        },
        timeout: 360000,
        success: function (response) {
            var res = Ext.decode(response.responseText);
            if (res.success) {
                Ext.Msg.alert(INFORMATION, SUCCESS);
                Search();
            }
            else {
                Ext.Msg.alert(INFORMATION, res.msg);
            }
            myMask.hide();
        },
        failure: function (response, opts) {
            if (response.timedout) {
                Ext.Msg.alert(INFORMATION, TIME_OUT);
            }
            myMask.hide();
        }
    });
}

function setValue(flag) {//flag:1為修改設值，2為查詢設值
    if (Ext.getCmp('isOverdue').getValue()) return;
    var site_id = Ext.getCmp("pro_site").getValue();
    var product_prefix = Ext.getCmp('product_prefix').getValue();
    var product_suffix = Ext.getCmp('product_suffix').getValue();
    var event_start = Ext.getCmp("start_from_date").getValue();
    var event_end = Ext.getCmp('start_to_date').getValue();
    if (!Ext.getCmp("pro_site").isValid()) return;

    if (site_id && !proname_store.findRecord("Site_Id", site_id)) {
        Ext.Msg.alert(MESSAGE, THIS_PRODUCT_NON_SITE_DATA); //提示,該商品無此站臺資料
        return;
    };
    Ext.Array.each(proname_store.data.items, function (record) {
        if ((site_id == 0 || record.data.Site_Id == site_id) && record.data.Flag != 3) {
            switch (flag) {
                case 1:
                    if (event_end) record.set('Event_End', event_end.getTime() / 1000);
                    if (record.data.Flag == 0) {
                        if (product_prefix) record.set('Product_Prefix', product_prefix);
                        if (product_suffix) record.set('Product_Suffix', product_suffix);
                        if (event_start) record.set('Event_Start', event_start.getTime() / 1000);
                    }
                    break;
                case 2:
                    if (record.data.Rid == 0) {
                        if (product_prefix) record.set('Product_Prefix', product_prefix);
                        if (product_suffix) record.set('Product_Suffix', product_suffix);
                        if (event_start) record.set('Event_Start', event_start.getTime() / 1000);
                        if (event_end) record.set('Event_End', event_end.getTime() / 1000);
                    }
                    ////查詢設值時不設狀態為[作用中]和[審核中]的活動結束時間
                    //if (event_end && record.data.Flag != 2 && record.data.Flag != 1) {
                    //    record.set('Event_End', event_end.getTime() / 1000);
                    //}
                    break;
            }
        }
    });
}