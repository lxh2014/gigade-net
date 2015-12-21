/*
新增：【新促銷】滿額滿件送禮    add by shuangshuang0420j and dongya0410j  201507030929
修改1：
修改2：
*/
var currentRecord = { data: {} };
var pageSize = 20;
var win;
var eventID = "";
var reset = 0;
var sort = "1";
Ext.Loader.setConfig({ enabled: true });
Ext.require([
    'Ext.data.*',
    'Ext.util.*',
    'Ext.view.View',
    'Ext.ux.DataView.DragSelector',
    'Ext.ux.DataView.LabelEditor'
]);

Ext.define('GIGADE.Master', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'row_id', type: 'int' },
        { name: 'cbjob_id', type: 'string' },
        { name: 'create_datetime', type: 'string' },
        { name: 'create_user', type: 'int' },
        { name: 'status', type: 'string' },
        { name: 'sta_id', type: 'string' }
    ]
});

var CbMasterStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'GIGADE.Master',
    proxy: {
        type: 'ajax',
        url: '/WareHouse/GetGetjobMasterList',
        //actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data',
        totalProperty: 'totalCount'
        }
    }
});
CbMasterStore.on('beforeload', function () {
   
    Ext.apply(CbMasterStore.proxy.extraParams, {
        startDate: Ext.getCmp('dateOne').getValue(),
        endDate: Ext.getCmp('dateTwo').getValue(),
        sta_id: Ext.getCmp('cbStatus').getValue(),
        jobStart: Ext.getCmp('IlocLeft').getValue(),
        jobEnd: Ext.getCmp('IlocRight').getValue()
    });
});


var DDLStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "單數", "value": "0" },
        { "txt": "雙數", "value": "1" }
    ]
});
var cbStatusStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "CNT", "value": "CNT" },
        { "txt": "UPD", "value": "UPD" },
        { "txt": "COM", "value": "COM" },
        { "txt": "END", "value": "END" }
    ]
});
Ext.onReady(function () {

    Ext.define("gigade.Vendor", {
        extend: 'Ext.data.Model',
        fields: [
            { name: "vendor_id", type: "string" },
            { name: "vendor_name_simple", type: "string" }
        ]
    });
    var VendorConditionStore = Ext.create('Ext.data.Store', {
        model: 'gigade.Vendor',
        autoLoad: false,
        proxy: {
            type: 'ajax',
            url: "/SendProduct/GetVendorName",
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });

    Ext.QuickTips.init();
    Ext.create('Ext.Viewport', {
        id: "index",
        autoScroll: true,
        width: document.documentElement.clientWidth,
        height: document.documentElement.clientHeight,
        layout: 'border',
        items: [{
            region: 'west',//左西
            xtype: 'panel',
            autoScroll: true,
            frame: false,
            width: 400,
            margins: '5 4 5 5',
            id: 'west-region-container',
            layout: 'anchor',
            items: InventoryLeft
        }
        ,
        {
            region: 'center',//中間
            id: 'center-region-container',
            xtype: 'panel',
            autoScroll: true,
            frame: false,
            layout: 'fit',
            width: 500,
            margins: '5 4 5 5',
            items: center
        }
        ],
        listeners: {
            resize: function () {
                Ext.getCmp("index").width = document.documentElement.clientWidth;
                Ext.getCmp("index").height = document.documentElement.clientHeight;
                this.doLayout();
            },
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        renderTo: Ext.getBody()
    });

});
//匯出詳細報表
ExportAllExcel = function () {
    var startDate = Ext.getCmp('dateOne').getValue();
    var endDate = Ext.getCmp('dateTwo').getValue();
    var sta_id = Ext.getCmp('cbStatus').getValue();
    var jobStart = Ext.getCmp('IlocLeft').getValue();
    var jobEnd = Ext.getCmp('IlocRight').getValue();
    window.open("/WareHouse/GetAllCountBook?startDate=" + Ext.Date.format(new Date(Ext.getCmp('dateOne').getValue()), 'Y-m-d H:i:s') + "&endDate=" + Ext.Date.format(new Date(Ext.getCmp('dateTwo').getValue()), 'Y-m-d H:i:s') + "&sta_id=" + sta_id + "&jobStart=" + jobStart + "&jobEnd=" + jobEnd);
}

function ExportDetailPDF(x) {

    var row = Ext.getCmp("detailist").getSelectionModel().getSelection();
    if (row.length <= 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else {
        //Ext.Msg.confirm(CONFIRM, Ext.String.format("確定要下採購單?", row.length), function (btn) {
        //    if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    //if (row[i].data.create_datetime.trim() == "") {
                    //    if (row[i].data.suggestPurchaseCount != "暫不需採購") {
                    rowIDs += row[i].data.row_id + ',';
                    //    }
                    //} else {
                    //    Ext.Msg.alert("提示", "所選的商品有已下採購單的!");
                    //    return;
                    //}
                }
               // alert(rowIDs);
                //if (rowIDs.trim() == "") {
                //    Ext.Msg.alert("提示", "所選的商品均不需採購!");
                //    return;
                //}
                //Ext.Ajax.request({
                //    url: '/ProductPurchase/AddItemIpo',
                //    method: 'post',
                //    params: {
                //        Items: rowIDs
                //    },
                //    success: function (form, action) {
                //        Ext.Msg.alert(INFORMATION, SUCCESS);
                //        SuggestPurchaseStore.load();

                //    },
                //    failure: function () {
                //        Ext.Msg.alert(INFORMATION, FAILURE);
                //        SuggestPurchaseStore.load();
                //    }
                //});
        //}
                window.open("/WareHouse/CountBookPDF?rowIDs=" + rowIDs);
     
    }

}
///匯出總報表
ExportAllExcel = function () {
    var startDate = Ext.getCmp('dateOne').getValue();
    var endDate = Ext.getCmp('dateTwo').getValue();
    var sta_id = Ext.getCmp('cbStatus').getValue();
    var jobStart = Ext.getCmp('IlocLeft').getValue();
    var jobEnd = Ext.getCmp('IlocRight').getValue();
    window.open("/WareHouse/GetAllCountBook?startDate=" + Ext.Date.format(new Date(Ext.getCmp('dateOne').getValue()), 'Y-m-d H:i:s') + "&endDate=" + Ext.Date.format(new Date(Ext.getCmp('dateTwo').getValue()), 'Y-m-d H:i:s') + "&sta_id=" + sta_id + "&jobStart=" + jobStart + "&jobEnd=" + jobEnd);
}

//左邊課程列表
var InventoryLeft = Ext.create('Ext.form.Panel', {
    frame: false,
    //width: 430,
    bodyPadding: 30,
    border: false,
    autoScroll: true,
    items: [

        {
            xtype: 'fieldcontainer',
            fieldLabel: "走道範圍",
            id: 'Iloc_all',
            labelWidth: 110,
            width: 300,
            autoScroll: true,
            combineErrors: true,
            layout: 'hbox',
            items: [
                {
                    xtype: "textfield",
                    id: 'startIloc',
                    name: 'startIloc',
                    allowBlank: true,
                    flex: 5
                },
                {
                    xtype: 'displayfield', margin: '0 0 0 0', value: "~"
                },
                {
                    xtype: "textfield",
                    id: 'endIloc',
                    name: 'endIloc',
                    flex: 5,
                    allowBlank: true
                }
            ]
        },
        {
            fieldLabel: "所在層數",
            xtype: "textfield",
            id: 'level',
            labelWidth: 110,
            name: 'level',
            allowBlank: true
        },
        {//會員群組和會員條件二擇一
            xtype: 'fieldcontainer',
            fieldLabel: '排序方式',
            combineErrors: true,
            labelWidth: 110,
            margins: '0 200 0 0',
            layout: 'hbox',
            defaults: {
                hideLabel: true
            },
            items: [
                {
                    xtype: 'radiofield',
                    name: 'usr',
                    id: "usr1",
                    listeners: {
                        change: function () {
                            sort = "1";
                            if (sort == "1") {
                                Ext.getCmp('Firstsd').show();
                                Ext.getCmp('Firstsd').setDisabled(true);
                            }
                        }
                    }
                },
                {
                    html: '<div>直線排序</div>',
                    frame: false,
                    border: false
                },
                {
                    xtype: 'displayfield',
                    width: 25
                },
                {
                    xtype: 'radiofield',
                    name: 'usr',
                    inputValue: "groupname",
                    //fieldLabel: 'Z排序',
                    id: "usr2",
                    checked: true,
                    listeners: {
                        change: function () {
                            sort = "0";
                            if (sort == "0") {
                                Ext.getCmp('Firstsd').show();
                                Ext.getCmp('Firstsd').setDisabled(false);
                            }
                        }
                    }
                },
                {
                    html: '<div>Z排序</div>',
                    frame: false,
                    border: false
                }
            ]
        },
        {
            xtype: 'fieldcontainer',
            fieldLabel: "",
            combineErrors: true,
            layout: 'hbox',
            items: [
                {
                    xtype: 'displayfield',
                    width: 115
                },
                {//直線排序
                    xtype: 'combobox',
                    id: 'Firstsd',
                    name: 'Firstsd',
                    fieldLabel: '',
                    store: DDLStore,
                    hiddenName: 'Firstsd_name',
                    colName: 'Firstsd_value',
                    displayField: 'txt',
                    valueField: 'value',
                    editable: false,
                    typeAhead: true,
                    width: 113,
                    labelWidth: 110,
                    forceSelection: false,
                    disabled: true,
                    value: "0"
                }
            ]
        },
          {
              xtype: 'checkboxgroup',
              fieldLabel: '是否買斷商品',
              labelWidth: 80,
              width: 350,
              margin: '0 10 0 0',
              id: 'prepaid',
              name: 'prepaid',
              columns: 1,
              items: [
                  { id: 'prepaidCheck', name: 'vtype', inputValue: '1', checked: true },
              ]
          },
        {
            fieldLabel: "供應商編號/簡稱",
            xtype: "textfield",
            id: 'vender',
            labelWidth: 110,
            name: 'vender',
            allowBlank: true
        },

        {
            xtype: 'button',
            text: "生成盤點工作",
            id: 'btnQuery',
            buttonAlign: 'center'
            ,
            handler: InsertInventory
        }
    ],
    renderTo: Ext.getBody()
});

//複選框列
var cbm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("edit").setDisabled(selections.length == 0);
        }
    }
});


var center = Ext.create('Ext.form.Panel', {
    id: 'center',
    autoScroll: true,
    border: false,
    frame: false,
    layout: { type: 'vbox', align: 'stretch' },
    defaults: { margin: '2 2 2 2' },
    items: [
        {
          
            title: '盤點薄資料',
            autoScroll: true,
            flex: 1.7,
            frame: false,
            items: [
                {
                    xtype: 'container',
                   
                    defaults: { margin: '0 5 5 10', labelWidth: 60, autoScroll: true, width: 1150 },
                    items: [
                             {
                                 xtype: 'fieldcontainer',
                                 combineErrors: true,
                                 margin: '3 5 5 10',
                                 fieldLabel: '盤點工作創建時間',
                                 labelWidth: 100,
                                 height: 30,
                                 layout: 'hbox',
                                 items: [
                                 {
                                     xtype: "datefield",
                                     labelWidth: 60,
                                     margin: '0 0 0 0',
                                     id: 'dateOne',
                                     name: 'dateOne',
                                     format: 'Y-m-d',
                                     allowBlank: false,
                                     editable: false,
                                     submitValue: true,
                                     //value: new Date(Tomorrow().setDate(Tomorrow().getDay() - 2)),
                                     value: new Date(Tomorrow().setMonth(Tomorrow().getMonth() - 1)),
                                     listeners: {
                                         select: function (a, b, c) {
                                             var start = Ext.getCmp("dateOne");
                                             var end = Ext.getCmp("dateTwo");
                                             var s_date = new Date(end.getValue());
                                             if (end.getValue() < start.getValue()) {
                                                 Ext.Msg.alert("提示", "開始時間不能大於結束時間！");
                                                 start.setValue(Tomorrow().setMonth(Tomorrow().getMonth() - 1));
                                             }
                                         },
                                         specialkey: function (field, e) {
                                             if (e.getKey() == Ext.EventObject.ENTER) {
                                                 Query();
                                             }
                                         }
                                     }
                                 },
                                 {
                                     xtype: 'displayfield', margin: '0 0 0 0', value: "~"
                                 },
                                 {
                                     xtype: "datefield",
                                     format: 'Y-m-d',
                                     id: 'dateTwo',
                                     name: 'dateTwo',
                                     margin: '0 0 0 0',
                                     editable: false,
                                     allowBlank: false,
                                     submitValue: true,
                                     value: Tomorrow(),
                                     listeners: {
                                         select: function (a, b, c) {
                                             var start = Ext.getCmp("dateOne");
                                             var end = Ext.getCmp("dateTwo");
                                             var s_date = new Date(start.getValue());
                                             if (end.getValue() < start.getValue()) {
                                                 Ext.Msg.alert("提示", "開始時間不能大於結束時間！");
                                                 end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                                             }
                                         },
                                         specialkey: function (field, e) {
                                             if (e.getKey() == Ext.EventObject.ENTER) {
                                                 Query();
                                             }
                                         }
                                     }
                                 }
                                 ,
                                  {
                                      xtype: 'combobox',
                                      id: 'cbStatus',
                                      name: 'cbStatus',
                                      fieldLabel: "",
                                      margin: '0 10 0 15',
                                      fieldLabel: '狀態',
                                      store: cbStatusStore,
                                      displayField: 'txt',
                                      editable: false,
                                      valueField: 'value',
                                      labelWidth: 50,
                                      value:'CNT'
                                  }
                                 ]
                             }, 

                      {
                          xtype: 'fieldcontainer',
                          fieldLabel: "料位區間",
                          id: 'Iloc_Interval',
                          labelWidth: 100,
                          width: 400,
                          height:30,
                          combineErrors: true,
                          layout: 'hbox',
                          items: [
                              {
                                  xtype: "textfield",
                                  id: 'IlocLeft',
                                  name: 'IlocLeft',
                                  allowBlank: true,
                                  flex: 5
                              },
                              {
                                  xtype: 'displayfield', margin: '0 0 0 0', value: "~"
                              },
                              {
                                  xtype: "textfield",
                                  id: 'IlocRight',
                                  name: 'IlocRight',
                                  flex: 5,
                                  allowBlank: true
                              },
                               {
                                   xtype: 'button',
                                   margin: '0 5 5 10',
                                   text: "查詢",
                                   id: 'btnSerch',
                                   iconCls: 'icon-search',
                                   handler: Search
                               }
                          ]
                      },
                     
                     
                    
                    ]
                }]
        }
        ,
           {
               // height: 530,
               //flex: 10.0,
               title: '盤點薄內容',
               xtype: 'gridpanel',
               id: 'detailist',
               autoScroll: true,
               //id: 'SuggestPurchaseView',
               //store: SuggestPurchaseStore,
               flex: 10,
               // width: document.documentElement.clientWidth,
               columnLines: true,
               // frame: true,
               frame: false,
               store: CbMasterStore,
               columns: [
                    { header: '序號', dataIndex: 'row_id', align: 'left', width: 60 },
                   { header: '工作代號', dataIndex: 'cbjob_id', align: 'left', width: 170 },
                    {
                        header: '料位區間', dataIndex: 'cbjob_id', align: 'left', width: 120, menuDisabled: true, sortable: false
                        , renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                            var t = record.data.cbjob_id.substring(6, 7);
                            if (t == "S" || t == "M" || t == "Z") {
                                var n = record.data.cbjob_id.substring(2, 4) + '~' + record.data.cbjob_id.substring(4, 6);
                                if (t == "S") {
                                    n += "(單)"
                                }
                                if (t == "M") {
                                    n += "(雙)"
                                }
                                return n;
                            } else {
                                return " ";
                            }

                        }

                    },
                    {
                        header: '創建時間', dataIndex: 'create_datetime', align: 'left', width: 120
                    },
                   {
                       header: '狀態', dataIndex: 'sta_id', align: 'left', width: 60
                   }

               ],
               tbar: [
                   {
                       xtype: 'button', text: "匯出總報表", id: 'exportExcel', icon: '../../../Content/img/icons/excel.gif', handler: ExportAllExcel
                   },
                   { xtype: 'button', text: '匯出詳細報表', id: 'edit', disabled: true, icon: '../../../Content/img/icons/excel.gif', handler: ExportDetailPDF },// handler: detailEdit,
               ],
               bbar: Ext.create('Ext.PagingToolbar', {
                   store: CbMasterStore,
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
               selModel: cbm
           }

    ]
})









function Search() {
    CbMasterStore.removeAll();
    CbMasterStore.load({
        params: {
            startDate: Ext.getCmp('dateOne').getValue(),
            endDate: Ext.getCmp('dateTwo').getValue(),
            sta_id: Ext.getCmp('cbStatus').getValue(),
            jobStart: Ext.getCmp('IlocLeft').getValue(),
            jobEnd: Ext.getCmp('IlocRight').getValue(),
            start:0
        }
    });
}

function Tomorrow() {
    var d;
    d = new Date();
    d.setDate(d.getDate() + 1);
    return d;
}
function InsertInventory()
{
    var checked = Ext.getCmp("prepaid").items;
    var check = "";

    if (checked.get(0).checked) {

        check = "1";
    } else {
        check = "0";
    }
    var myMask = new Ext.LoadMask(Ext.getBody(), {
        msg: 'Loading...'
    });
    myMask.show();
    Ext.Ajax.request({
        url: '/WareHouse/GetCountBook2',
        params: {
            level:Ext.getCmp('level').getValue(),
            startIloc: Ext.getCmp('startIloc').getValue(),
            endIloc:  Ext.getCmp('endIloc').getValue(),
            sort: sort,
            firstsd: Ext.getCmp('Firstsd').getValue(),
            vender:  Ext.getCmp('vender').getValue(),
            prepaid: check
        },
        timeout: 360000,
        success: function (response) {
            var res = Ext.decode(response.responseText);
            if (res.success) {
                myMask.hide();
                if (res.msg == "1") {
                    if (Ext.getCmp('startIloc').getValue().trim() != "") {
                        Ext.getCmp('IlocLeft').setValue(Ext.getCmp('startIloc').getValue());
                    }
                    if (Ext.getCmp('endIloc').getValue().trim() != "") {
                        Ext.getCmp('IlocRight').setValue(Ext.getCmp('endIloc').getValue());
                    }
                    Ext.getCmp('dateOne').setValue(new Date());
                    Ext.getCmp('dateTwo').setValue(new Date());
                    Ext.getCmp('cbStatus').setValue('CNT');

                    CbMasterStore.load({
                        params: {
                            startDate: Ext.getCmp('dateOne').getValue(),
                            endDate: Ext.getCmp('dateTwo').getValue(),
                            sta_id:Ext.getCmp('cbStatus').getValue(),
                            jobStart: Ext.getCmp('IlocLeft').getValue(),
                            jobEnd: Ext.getCmp('IlocRight').getValue(),
                            start: 0
                        }
                    });
                   // Ext.Msg.alert(INFORMATION, "1");
                }
                else if (res.msg == "2")
                {
                    Ext.Msg.alert(INFORMATION, "此範圍內暫無數據!");
                }
                //else {
                   
                //}
             
               
              
            }
            else {
                Ext.Msg.alert(INFORMATION, FAILURE);
                myMask.hide();
            }

        },
        failure: function (response, opts) {
            if (response.timedout) {
                Ext.Msg.alert(INFORMATION, TIME_OUT);
            }
            myMask.hide();
        }
    });
}



