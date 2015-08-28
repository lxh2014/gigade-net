
var CallidForm;
var pageSize = 25;
/**********************************************************************群組管理主頁面**************************************************************************************/
//群組管理Model
Ext.apply(Ext.form.field.VTypes, {
    daterange: function (val, field) {
        var date = field.parseDate(val);

        if (!date) {
            return false;
        }
        this.dateRangeMax = null;
        this.dateRangeMin = null;
        if (field.startDateField && (!this.dateRangeMax || (date.getTime() != this.dateRangeMax.getTime()))) {
            var start = field.up('form').down('#' + field.startDateField);
            start.setMaxValue(date);
            //start.validate();
            this.dateRangeMax = date;
        } else if (field.endDateField && (!this.dateRangeMin || (date.getTime() != this.dateRangeMin.getTime()))) {
            var end = field.up('form').down('#' + field.endDateField);
            end.setMinValue(date);
            //end.validate();
            this.dateRangeMin = date;
        }
        /*  
         * Always return true since we're only using this vtype to set the  
         * min/max allowed values (these are tested for after the vtype test)  
         */
        return true;
    },

    daterangeText: '開始時間必須小於結束時間'
});
Ext.define('gigade.Fares', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "group_id", type: "int" },
        { name: "group_name", type: "string" },
        { name: "tax_id", type: "string" },
        { name: "ip", type: "string" },
        { name: "list", type: "string" },
        { name: "gift_bonus", type: "int" },
        { name: "bonus_rate", type: "int" },
        { name: "bonus_expire_day", type: "int" },
        { name: "screatedate", type: "string" },
        { name: "image_name", type: "string" },
        { name: "eng_name", type: "string" },
        { name: "check_iden", type: "int" },
        { name: "group_category", type: "int" }

    ]
});

var FaresStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Fares',
    proxy: {
        type: 'ajax',
        //url:controller/fangfaming
        url: '/Member/GetVipUserGroupList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdFgroup").down('#edit').setDisabled(selections.length == 0);
            //            Ext.getCmp("gdFgroup").down('#remove').setDisabled(selections.length == 0);
            //            Ext.getCmp("gdFgroup").down('#auth').setDisabled(selections.length == 0);
            //            Ext.getCmp("gdFgroup").down('#callid').setDisabled(selections.length == 0);
        }
    }
});

FaresStore.on('beforeload', function () {
    Ext.apply(FaresStore.proxy.extraParams, {
        dateOne: Ext.getCmp('dateOne').getValue(),
        dateTwo: Ext.getCmp('dateTwo').getValue()
    })
})
var DDLStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "所有會員資料", "value": "0" },
        { "txt": "電子信箱", "value": "1" },
        { "txt": "會員姓名", "value": "2" },
        { "txt": "手機號碼", "value": "3" }
    ]
});

//var vipUserTpl = new Ext.XTemplate(
//    '<a href="/VipUserGroup/VipUserGroupAddList?id={group_id}">{list}</a>'
//);

Ext.onReady(function () {
    var gdFgroup = Ext.create('Ext.grid.Panel', {
        id: 'gdFgroup',
        store: FaresStore,
        width: document.documentElement.clientWidth,
        columnLines: true,       
        frame: true,
        columns: [
            {
                header: "名單編號", dataIndex: 'group_id', width: 60, align: 'center'
            },
            { header: "名單名稱", dataIndex: 'group_name', width: 150, align: 'center' },
            { header: "統編", dataIndex: 'tax_id', width: 100, align: 'center' },
            {
                header: "企業網址", dataIndex: 'ip', width: 100, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.tax_id != '' && record.data.eng_name != '' && record.data.group_category != 0) {
                        if (value != '') {
                            if (value.split('.')[0] == '192') {
                                return Ext.String.format('<a href="http://www.gimg.tw/btoe/{0}" target="bank">聯結</a>', record.data.eng_name);
                            }
                            else {
                                return Ext.String.format('<a href="http://www.gigade100.com/btoe/{0}" target="bank">聯結</a>', record.data.eng_name);

                            }
                        }
                    }
                    else {
                        return Ext.String.format('<a href="/Member/VipUserGroupList" target="_self">聯結</a>');
                    }
                }
            },
            {
                header: "名單數", id: 'count', hidden: true, dataIndex: 'list', width: 100, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return Ext.String.format('<a href="/Member/VipUserList?id={0}" target="_self">{1}</a>', record.data.group_id, value);
                }
            },
            {
                header: "其它功能", dataIndex: '', id: 'other', width: 200, align: 'center', hidden: true,
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.check_iden == 0) {
                        return Ext.String.format('<a href="/Member/VipUserImport?id={0}&name={1}" target="_self">匯入</a>', record.data.group_id, record.data.group_name);
                    }
                    if (record.data.check_iden == 1) {
                        return Ext.String.format('<a href="/Member/VipUserImport?id={0}&name={1}" target="_self">匯入</a>&nbsp;&nbsp;&nbsp;<a href="/Member/VipUserImport?id={0}&name={1}&check={2}" target="_self">匯入員工編號</a>', record.data.group_id, record.data.group_name, record.data.check_iden);
                    }


                }
            },
            { header: "註冊贈送購物金額", dataIndex: 'gift_bonus', width: 100, align: 'center' },
            { header: "購物贈送購物金倍率", dataIndex: 'bonus_rate', width: 150, align: 'center' },
            { header: "購物贈送購物金有效天數", dataIndex: 'bonus_expire_day', width: 150, align: 'center' },
            { header: "建立時間", dataIndex: 'screatedate', width: 180, align: 'center' }
        ],
        tbar: [
           { xtype: 'button', text: ADD, id: 'add', hidden: true, iconCls: 'icon-user-add', handler: onAddClick },
           { xtype: 'button', text: EDIT, id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
           '->',
          {
              xtype: "datefield",
              fieldLabel: "建立時間",
              id: 'dateOne',
              name: 'dateOne',
              format: 'Y-m-d',
              allowBlank: false,
              editable: false,             
              submitValue: true,
              value: new Date(Tomorrow().setMonth(Tomorrow().getMonth() - 1)),
              listeners: {
                  select: function (a, b, c) {
                      var start = Ext.getCmp("dateOne");
                      var end = Ext.getCmp("dateTwo");
                      if (end.getValue() == null) {
                          end.setValue(setNextMonth(start.getValue(), 1));
                      }
                      else if (end.getValue() < start.getValue()) {
                          Ext.Msg.alert(INFORMATION, DATA_TIP);
                          end.setValue(setNextMonth(start.getValue(), 1));
                      }
                      else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                          // Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                          end.setValue(setNextMonth(start.getValue(), 1));
                      }
                  }
              }
          }, '~', {
              xtype: "datefield",
              format: 'Y-m-d',
              id: 'dateTwo',
              name: 'dateTwo',
              allowBlank: false,
              editable: false,            
              submitValue: true,
              value: Tomorrow(),
              listeners: {
                  select: function (a, b, c) {
                      var start = Ext.getCmp("dateOne");
                      var end = Ext.getCmp("dateTwo");
                      if (start.getValue() != "" && start.getValue() != null) {
                          if (end.getValue() < start.getValue()) {
                              Ext.Msg.alert(INFORMATION, DATA_TIP);
                              start.setValue(setNextMonth(end.getValue(), -1));
                          }
                          else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                              // Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                              start.setValue(setNextMonth(end.getValue(), -1));
                          }
                      }
                      else {
                          start.setValue(setNextMonth(end.getValue(), -1));
                      }
                  }
              }
          },
            {
                text: SEARCH,
                iconCls: 'icon-search',
                id: 'btnQuery',
                handler: Query
            }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: FaresStore,
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
        items: [gdFgroup],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdFgroup.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    // FaresStore.load({ params: { start: 0, limit: 25 } });
    if (document.getElementById("valet_service").value == 1) {
        Ext.getCmp('other').show();
        Ext.getCmp('add').show();
    }
});

/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {

    //addWin.show();
    editFunction(null, FaresStore);

}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("gdFgroup").getSelectionModel().getSelection();
    //alert(row[0]);
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], FaresStore);
    }
}

function Tomorrow() {
    var d;
    d = new Date();                             // 创建 Date 对象。                               // 返回日期。
    d.setDate(d.getDate() + 1);
    return d;
}
setNextMonth = function (source, n) {
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
function Query() {

    FaresStore.removeAll();
    Ext.getCmp("gdFgroup").store.loadPage(1, {
        params: {
            dateOne: Ext.getCmp('dateOne').getValue(),
            dateTwo: Ext.getCmp('dateTwo').getValue()
        }
    });
}




