var pageSize = 25;
var comType = false;
Ext.define('gigade.InspectionReport', {
    extend: 'Ext.data.Model',
    fields: [
    { name: "rowID", type: "int" },
      { name: "brand_id", type: "int" },
      { name: "brand_name", type: "string" },
         { name: "product_name", type: "string" },
    { name: "product_id", type: "int" },
    { name: "certificate_type1", type: "string" },
    { name: "certificate_type2", type: "string" },
        { name: "certificate_type1_name", type: "string" },
    { name: "certificate_type2_name", type: "string" },
    { name: "certificate_expdate", type: "string" },
    { name: "certificate_desc", type: "string" },
    { name: "certificate_filename", type: "string" },
    { name: "k_user", type: "int" },
    { name: "k_date", type: "string" },
    { name: "m_user", type: "int" },
    { name: "m_date", type: "string" },
    { name: "create_user", type: "string" },
    { name: "update_user", type: "string" },
      { name: "certificate_filename_string", type: "string" },
    {name:'sort',type:'int'},
    ]
});
var InspectionReportStore = Ext.create('Ext.data.Store', {
    model: 'gigade.InspectionReport',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/InspectionReport/InspectionReportList',
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
            var row = Ext.getCmp("InspectionReport").getSelectionModel().getSelection();
            Ext.getCmp("InspectionReport").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("InspectionReport").down('#remove').setDisabled(selections.length == 0);
        }
    }
});
InspectionReportStore.on('beforeload', function () {
    Ext.apply(InspectionReportStore.proxy.extraParams,
        {
            brand: Ext.getCmp('Brand_Id').getValue(),
            name_code: Ext.getCmp('name_code').getValue(),
            certificate_type1: Ext.getCmp('certificate_type1_1').getValue(),
            certificate_type2: Ext.getCmp('certificate_type2_2').getValue(),
            start_time: Ext.getCmp('start_time').getValue(),
            end_time: Ext.getCmp('end_time').getValue(),
            last_day: Ext.getCmp('last_day').getValue(),
            search_date: Ext.getCmp('search_date').getValue(),
        });
});


Ext.define("gigade.VendorBrand", {
    extend: 'Ext.data.Model',
    fields: [
            { name: "Brand_Id", type: "string" },
        { name: "Brand_Name", type: "string" }
    ],
});

Ext.define('GIGADE.CertificateModel', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'certificate_categoryname', type: 'string' },
        { name: 'frowID', type: 'int' }
    ]
})

var CertificateStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.CertificateModel',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/InspectionReport/GetGroup",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }

    }
});
Ext.define('GIGADE.CertificateModel2', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'certificate_categoryname', type: 'string' },
        { name: 'frowID', type: 'int' }
    ]
})

var CertificateStore2 = Ext.create('Ext.data.Store', {
    model: 'GIGADE.CertificateModel2',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: "/InspectionReport/GetType2Group",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }

    }
});
CertificateStore2.on('beforeload', function () {
    Ext.apply(CertificateStore2.proxy.extraParams,
        {
            ROWID: Ext.getCmp('certificate_type1_1').getValue(),
        });
});
var brandStore = Ext.create("Ext.data.Store", {
    model: 'gigade.VendorBrand',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/InspectionReport/BrandStore",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }

    }
});
var DateStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "所有日期", "value": "0" },
        { "txt": "有效期限", "value": "1" }
    ]
});

Ext.onReady(function () {

    var searchFrm = Ext.create('Ext.form.Panel', {
        id: 'searchFrm',
        bodyPadding: '15',
        border: 0,
        width: document.documentElement.clientWidth,
        items: [
            {
                xtype: 'combobox',
                id: 'Brand_Id',
                name: 'Brand_Id',
                labelWidth: 120,
                queryMode: 'local',
                typeAhead: true,
                store: brandStore,
                displayField: 'Brand_Name',
                valueField: 'Brand_Id',
                fieldLabel: '品牌',
                listeners: {
                    blur: function () {
                        if (Ext.getCmp('Brand_Id').getValue() != "undefined"&& Ext.getCmp('Brand_Id').getValue()!="") {
                            Ext.Ajax.request({
                                url: '/InspectionReport/GetBrandID',
                                params: {
                                    brand_name: Ext.getCmp('Brand_Id').getRawValue(),
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(form.responseText);
                                    if (result.success) {
                                        Ext.getCmp('Brand_Id').setValue(result.brand_id);
                                    }
                                    else {
                                       // Ext.Msg.alert("提示信息", "無此品牌或該品牌被隱藏！");
                                        Ext.getCmp('Brand_Id').setValue("");
                                    }
                                }
                            });
                        }
                      
                        }
                    }
            },
            {
                xtype: 'textfield',
                id: 'name_code',
                name: 'name_code',
                labelWidth: 120,
                fieldLabel: '商品名稱/編號',
                listeners: {
                    specialkey: function (field, e) {
                        if (e.getKey() == Ext.EventObject.ENTER) {
                            Query();
                        }
                    }
                }
            },
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                    {
                        xtype: 'combobox',
                        id: 'certificate_type1_1',
                        editable: false,
                        queryMode: 'remote',
                        store: CertificateStore,
                        valueField: 'frowID',
                        displayField:'certificate_categoryname',
                        labelWidth: 120,
                        fieldLabel: '證書類別',
                        listeners: {
                            select: function (combo,record) {
                                var m = Ext.getCmp("certificate_type2_2");
                                m.clearValue();
                                CertificateStore2.removeAll();
                                comType = true;
                            }
                        },
                    },
                    {
                        xtype: 'combobox',
                        id: 'certificate_type2_2',
                        editable: false,
                        store: CertificateStore2,
                        valueField: 'frowID',
                        displayField: 'certificate_categoryname',
                        listeners: {
                            beforequery: function (qe) {
                                if (comType) {
                                    delete qe.combo.lastQuery;
                                    CertificateStore2.load({
                                        params: {
                                            ROWID: Ext.getCmp('certificate_type1_1').getValue(),
                                        }
                                    });
                                    comType = false;
                                }
                            }
                        }
                    }
                ],
            },
   {
       xtype: 'fieldcontainer',
       layout: 'hbox',
       items: [
           {
               xtype: 'combobox',
               fieldLabel: '查詢日期',
               store: DateStore,
               id:'search_date',
               valueField: 'value',
               editable: false,
               labelWidth: 120,
               value:'0',
               displayField: 'txt',
               //margin: '0 0 0 10',

           },
           {
               xtype: 'datetimefield',
               id: 'start_time',
               name: 'start_time',
               format: 'Y-m-d H:i:s',
               time: { hour: 00, min: 00, sec: 00 },
               editable: false,
               labelWidth: 120,
               value: Tomorrow(),
               listeners: {
                   select: function (a, b, c) {
                       var start = Ext.getCmp("start_time");
                       var end = Ext.getCmp("end_time");
                       if (end.getValue() < start.getValue()) {
                           var start_date = start.getValue();
                           Ext.getCmp('end_time').setValue(new Date(start_date.getFullYear(), start_date.getMonth() + 1, start_date.getDate(), 23, 59, 59));
                       }
                   }
               }
           },
           {
               xtype: 'datetimefield',
               id: 'end_time',
               name: 'end_time',
               format: 'Y-m-d H:i:s',
               time: { hour: 23, min: 59, sec: 59 },
               editable: false,
               value: setNextMonth(Tomorrow(), 1),
               listeners: {
                   select: function (a, b, c) {
                       var start = Ext.getCmp("start_time");
                       var end = Ext.getCmp("end_time");
                       if (end.getValue() < start.getValue()) {//開始時間大於了結束時間
                           var end_date = end.getValue();
                           Ext.getCmp('start_time').setValue(new Date(end_date.getFullYear(), end_date.getMonth() - 1, end_date.getDate()));
                       }
                   }
               }
           }
       ],
   },
  {
      xtype: 'fieldcontainer',
      layout: 'hbox',
      items: [
             {
                 xtype: 'displayfield',
                 value: '有效期剩下天數<=',
                 fieldLabel:'',
                 labelWidth: 120,
                
             },
             {
                 xtype: 'numberfield',
                 id: 'last_day',
                 allowDecimals: false,
                 minValue:'1',
                 margin:'0 0 0 11',
                 name: 'last_day',
                 listeners: {
                     specialkey: function (field, e) {
                         if (e.getKey() == Ext.EventObject.ENTER) {
                             Query();
                         }
                     }
                 }
             },
                {
                    xtype: 'displayfield',
                    value: '天',
                },
      ],
  },

        ],
        buttonAlign:'left',
        buttons: [
            {
                text: '查詢',
                handler: function () {
                    Query();
                }
            },
            {

                text: '重置',
                handler: function () {
                    this.up('form').getForm().reset();
                }
            },
        ],
    });


    var InspectionReport = Ext.create('Ext.grid.Panel', {
        id: 'InspectionReport',
        store: InspectionReportStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex:8.1,
        columns: [
              { header: "流水號", dataIndex: 'rowID', width: 150, align: 'center' },
        { header: "品牌編號", dataIndex: 'brand_id', width: 150, align: 'center' },
        { header: "品牌名稱", dataIndex: 'brand_name', width: 150, align: 'center' },
        { header: "商品編號", dataIndex: 'product_id', width: 120, align: 'center' },
        { header: "商品名稱", dataIndex: 'product_name', width: 120, align: 'center' },
        { header: "證書大類", dataIndex: 'certificate_type1_name', width: 120, align: 'center' },
        { header: "證書小類", dataIndex: 'certificate_type2_name', width: 120, align: 'center' },
            { header: "排序", dataIndex: 'sort', width: 50, align: 'center' },
        { header: "有效期限", dataIndex: 'certificate_expdate', width: 160, align: 'center' },
        { header: "說明", dataIndex: 'certificate_desc', width: 120, align: 'center' },
        {
            header: "檔案", dataIndex: 'certificate_filename_string', width: 120, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return '<a target="_blank", href="' + record.data.certificate_filename_string + '"><img width="50px" height="50px" src="' + record.data.certificate_filename_string + '" /></a>'
    },
        },
           { header: "創建人員", dataIndex: 'create_user', width: 120, align: 'center' },
        { header: "創建日期", dataIndex: 'k_date', width: 160, align: 'center' },
        { header: "異動人員", dataIndex: 'update_user', width: 120, align: 'center' },
        { header: "異動日期", dataIndex: 'm_date', width: 160, align: 'center' },
     
        ],
        tbar: [
        { xtype: 'button', text: "新增", id: 'add', iconCls: 'ui-icon ui-icon-user-add', handler: onAddClick },
        { xtype: 'button', text: "編輯", id: 'edit', iconCls: 'ui-icon ui-icon-user-edit', disabled: true, handler: onEditClick },
        { xtype: 'button', text: "刪除", id: 'remove', iconCls: 'ui-icon ui-icon-user-delete', disabled: true, handler: onRemoveClick },
         { xtype: 'button', id: 'Import', text: "匯出Excel", icon: '../../../Content/img/icons/excel.gif', hidden: false, handler: Export },
        '->',
        { text: ' ' }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: InspectionReportStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: "當前顯示記錄" + ': {0} - {1}' + "總計" + ': {2}',
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
        items: [searchFrm,InspectionReport],
        renderTo: Ext.getBody(),
        listeners: {
            resize: function () {
                InspectionReport.width = document.documentElement.clientWidth;

                this.doLayout();
            }
        }
    });
    ToolAuthority();
  //  InspectionReportStore.load({ params: { start: 0, limit: 25 } });
});

function Query() {
    //var brand= Ext.getCmp('Brand_Id').getValue();
    //var   name_code= Ext.getCmp('name_code').getValue();
    //var  certificate_type1= Ext.getCmp('certificate_type1_1').getValue();
    //var  certificate_type2= Ext.getCmp('certificate_type2_2').getValue();
    //var  start_time= Ext.getCmp('start_time').getValue();
    //var   end_time= Ext.getCmp('end_time').getValue();
    //var last_day = Ext.getCmp('last_day').getValue();

        Ext.getCmp("InspectionReport").store.loadPage(1, {
            params: {
                brand: Ext.getCmp('Brand_Id').getValue(),
                name_code: Ext.getCmp('name_code').getValue(),
                certificate_type1: Ext.getCmp('certificate_type1_1').getValue(),
                certificate_type2: Ext.getCmp('certificate_type2_2').getValue(),
                start_time: Ext.getCmp('start_time').getValue(),
                end_time: Ext.getCmp('end_time').getValue(),
                last_day: Ext.getCmp('last_day').getValue(),
                search_date: Ext.getCmp('search_date').getValue(),
            }
        });

}

//*********新增********//
onAddClick = function () {
    editFunction(null, InspectionReportStore);
}
//匯出
ImportExcel = function () {
    ImportFunction();
}
function Export() {
    Ext.MessageBox.show({
        msg: '正在匯出，請稍後....',
        width: 300,
        wait: true
    });
    Ext.Ajax.request({
        url: "/InspectionReport/Export",
        timeout: 900000,
        params: {
            brand: Ext.getCmp('Brand_Id').getValue(),
            name_code: Ext.getCmp('name_code').getValue(),
            certificate_type1: Ext.getCmp('certificate_type1_1').getValue(),
            certificate_type2: Ext.getCmp('certificate_type2_2').getValue(),
            start_time: Ext.getCmp('start_time').getValue(),
            end_time: Ext.getCmp('end_time').getValue(),
            last_day: Ext.getCmp('last_day').getValue(),
            search_date: Ext.getCmp('search_date').getValue(),
        },
        success: function (form, action) {
            Ext.MessageBox.hide();
            var result = Ext.decode(form.responseText);
            if (result.success) {

                window.location = '../../ImportUserIOExcel/' + result.ExcelName;
              
            } else {
                Ext.MessageBox.hide();
                Ext.Msg.alert(INFORMATION, "匯出失敗或沒有數據,請先搜索查看!");
            }
        }
    });
}

//*********編輯********//
onEditClick = function () {
    var row = Ext.getCmp("InspectionReport").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert("提示信息", "沒有選擇一行！");
    }
    else if (row.length > 1) {
        Ext.Msg.alert("提示信息", "只能選擇一行！");
    } 
    else {
        editFunction(row[0], InspectionReportStore);
    }
}

//*********刪除********//
onRemoveClick = function () {
    
    var row = Ext.getCmp("InspectionReport").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert("提示信息", "沒有選擇一行！");
    }
    else {
        var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
        myMask.show();
        Ext.Msg.confirm("確認信息", Ext.String.format("刪除選中 {0} 條數據？", row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.rowID + ',' + row[i].data.brand_id + "," + row[i].data.product_id + "," + row[i].data.certificate_filename + "∑";
                }
                Ext.Ajax.request({
                    url: '/InspectionReport/DeleteInspectionRe',
                    method: 'post',
                    params: { rowID: rowIDs },
                    success: function (form, action) {
                        myMask.hide();
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            Ext.Msg.alert("提示信息", "刪除成功！");
                        }
                        else {
                            Ext.Msg.alert("提示信息", "刪除失敗！");
                        }
                        InspectionReportStore.load();
                    },
                    failure: function () {
                        myMask.hide();
                        Ext.Msg.alert("提示信息", "刪除失敗！");
                        InspectionReportStore.load();
                    }
                });
            }
            else {
                myMask.hide();
            }
        });
    }
}
function NextMonth() {
    var d;
    d = new Date();                             // 创建 Date 对象。
    d.setDate(d.getMonth() + 1);
    return d;
}
//******更改狀態******//
function UpdateActive(row_id) {
    var activeValue = $("#img" + row_id).attr("hidValue");
    $.ajax({
        url: "/MailGroup/UpMailGroupStatus",
        data: {
            "id": row_id,
            "active": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            if (activeValue == 1) {
                $("#img" + id).attr("hidValue", 0);
                $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
                MailGroupStore.load();
            } else {
                $("#img" + id).attr("hidValue", 1);
                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                MailGroupStore.load();
            }
        },
        error: function (msg) {
            Ext.Msg.alert("提示信息", "操作失敗");
            MailGroupStore.load();
        }
    });
}
function Tomorrow() {
    var d;
    var dt;
    var s = "";
    d = new Date();                             // 创建 Date 对象。
    s += d.getFullYear() + "/";                     // 获取年份。
    s += (d.getMonth() + 1) + "/";              // 获取月份。
    s += d.getDate();
    dt = new Date(s);
    //dt.setDate(dt.getDate() + 1);
    return dt;                                 // 返回日期。
}

function setNextMonth(source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0) {
        s.setHours(0, 0, 0);
    }
    else if (n > 0) {
        s.setHours(23, 59, 59);
    }
    return s;
}
