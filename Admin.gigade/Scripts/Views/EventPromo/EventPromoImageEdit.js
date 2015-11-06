var brand_name = "";
var add = false;
var excels = ['jpg', 'png', 'jpeg', 'gif']; //['xls', 'xlsx'];
var image = "";
var del_ids = "";
var del_names = "";
Ext.apply(Ext.form.field.VTypes, {
    imageFilter: function (val, field) {
        var type = val.split('.')[val.split('.').length - 1].toLocaleLowerCase();
        for (var i = 0; i < excels.length; i++) {
            if (excels[i] == type) {
                return true;
            }
        }
        return false;
    },
    imageFilterText: '文件格式錯誤',
});

function editFunction(row, store,multi) {
    Ext.define('gigade.PromotionBannerRelation', {
        extend: 'Ext.data.Model',
        fields: [
        { name: "pb_id", type: "int" },
        { name: "brand_id", type: "int" },
        { name: "brand_name", type: "string" }
        ]
    });
    var pbBrandStore = Ext.create('Ext.data.Store', {
        model: 'gigade.PromotionBannerRelation',
        autoLoad: false,
        proxy: {
            type: 'ajax',
            url: "/EventPromo/GetRelationList",
            autoLoad: true,
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });
    pbBrandStore.on('beforeload', function () {
        Ext.apply(pbBrandStore.proxy.extraParams,
        {
            pb_id: row.data.pb_id,
        });
    });
    var brand_Grid = Ext.create('Ext.grid.Panel', {
        id: 'brand_Grid',
        store: pbBrandStore,
        height: 147,
        columnLines: true,
        scroll: false,
        autoScroll: true,
        viewConfig: {
            style: { overflow: 'auto', overflowX: 'hidden' }
        },
        frame: true,
        columns: [
        {
            header: '刪除', xtype: 'actioncolumn', flex: 1, align: 'center',
            items: [
            {
                icon: '../../../Content/img/icons/cross.gif',
                tooltip: '刪除',
                handler: function (grid, rowIndex, colIndex) {
                    Ext.Msg.confirm("注意", "確認刪除？", function (btn) {
                        if (btn === 'yes') {
                            var i = rowIndex;
                            del_ids += Ext.getCmp("brand_Grid").store.data.items[i].data.brand_id + "|";
                            del_names += Ext.getCmp("brand_Grid").store.data.items[i].data.brand_name + "|";
                            pbBrandStore.removeAt(rowIndex);
                        }
                    });
                }
            }
            ]
        },
        {
            header: '品牌編號', dataIndex: 'brand_id', flex: 1, align: 'center'
        },

        {
            header: '品牌名稱', dataIndex: 'brand_name', flex: 3, align: 'center'
        }
        ]
    });
    var frm = Ext.widget('form', {
        id: 'frm',
        plain: true,
        height: 380,
        frame: true,
        layout: 'anchor',
        url: '/EventPromo/AddorEdit',
        items: [
        {
            xtype: 'form',
            id: 'add_bid',
            hidden: false,
            layout: 'anchor',
            border: 0,
            frame: true,
            allowBlank: false,
            defaults: {
                anchor: '100%'
            },
            items: [
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                id: 'fcon_brand',
                margin: '0 0 0 15',
                items: [
                {
                    xtype: 'displayfield',
                    fieldLabel: '圖片編號',
                    name: 'pb_id',
                    id: 'pb_id',
                    hidden: true
                },
                {
                    xtype: 'textfield',
                    fieldLabel: '品牌編號',
                    name: 'b_id',
                    id: 'b_id',
                    width: 200,
                    labelWidth: 60,
                    regex: /^([0-9]{1,9})$/,
                    listeners: {
                        specialkey: function (field, e) {
                            if (e.getKey() == e.ENTER) {
                                addBrd();
                            }
                        }
                    }
                },
                {
                    xtype: 'button',
                    text: '新增',
                    margin: '0 0 5 10',
                    width: 60,
                    handler: addBrd
                }
                ]

            },
             {
                 xtype: 'panel',
                 margin: '0 0 0 15',
                 hidden: multi == 1 ? true : false,
                 bodyStyle: "color:red;padding:5px;background:#DFE9F6",
                 border: false,
                 html: "注意事項：請勿添加已有促銷圖片的品牌編號！"
             },
            ]
        },
        brand_Grid,
        {
            xtype: 'form',
            id: 'img_info',
            hidden: false,
            layout: 'anchor',
            border: 0,
            frame: true,
            allowBlank: false,
            defaults: {
                anchor: '100%'
            },
            items: [
            {
                xtype: 'filefield',
                fieldLabel: '促銷圖片檔案',
                id: 'pb_image',
                name: 'pb_image',
                msgTarget: 'side',
                buttonText: '選擇...',
                margin: '5 0 5 15',
                anchor: '90%',
                flex: 1,
                submitValue: true,
                allowBlank: false,
                fileUpload: true,
                vtype: 'imageFilter'
            },
            {
                xtype: 'textfield',
                fieldLabel: '圖片連結地址',
                id: 'pb_image_link',
                name: 'pb_image_link',
                vtype: 'url',
                allowBlank: true,
                hidden: false,
                anchor: '90%',
                margin: '5 0 0 15',
            },
            {
                xtype: "datetimefield",
                fieldLabel: '顯示開始時間',
                id: 'pb_startdate',
                name: 'pb_startdate',
                format: 'Y-m-d H:i:s',
                anchor: '90%',
                margin: '5 0 0 15',
                submitValue: true,
                enable: false,
                editable: false,
                time: { hour: 00, min: 00, sec: 00 },
                value: Tomorrow(),
                listeners: {
                    select: function (a, b, c) {
                        var start = Ext.getCmp("pb_startdate");
                        var end = Ext.getCmp("pb_enddate");
                        var s_date = new Date(start.getValue());
                        if (end == null) {
                            end.setValue(setNextMonth(start.getValue(), 1));
                        }
                        if (end.getValue() < start.getValue()) {
                            Ext.Msg.alert(INFORMATION, '開始時間不能大於結束時間');
                            end.setValue(setNextMonth(start.getValue(), 1));
                        }
                    }
                }
            },
            {
                xtype: "datetimefield",
                id: 'pb_enddate',
                name: 'pb_enddate',
                format: 'Y-m-d H:i:s',
                fieldLabel: '顯示結束時間',
                margin: '5 0 0 15',
                anchor: '90%',
                submitValue: true,
                editable: false,
                time: { hour: 23, min: 59, sec: 59 },
                value: setNextMonth(Tomorrow(), 1),
                listeners: {
                    select: function (a, b, c) {
                        var start = Ext.getCmp("pb_startdate");
                        var end = Ext.getCmp("pb_enddate");
                        var s_date = new Date(end.getValue());
                        if (start == null) {
                            start.setValue(setNextMonth(end.getValue(), -1));
                        }
                        if (end.getValue() < start.getValue()) {
                            Ext.Msg.alert(INFORMATION, '結束時間不能小於開始時間');
                            start.setValue(setNextMonth(end.getValue(), -1));
                        }
                    }
                }
            }
            ]
        }
        ],
        buttons: [{
            text: SAVE,
            id: 'save',
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (Ext.getCmp('pb_image') == "" || Ext.getCmp('pb_startdate') == null || Ext.getCmp('pb_enddate') == null)
                {
                    Ext.getCmp("save").setDisabled(true); 
                }
                if (form.isValid()) {
                    var InsertValues = "";
                    var brand_con = Ext.getCmp("brand_Grid").store.data.items;
                    for (var a = 0; a < brand_con.length; a++) {
                        var vb_id = brand_con[a].get("brand_id");
                        InsertValues += vb_id + ',';
                    }
                    var pb_id = Ext.htmlEncode(Ext.getCmp('pb_id').getValue());
                    image = Ext.htmlEncode(Ext.getCmp('pb_image').getValue());
                    var image_link = Ext.htmlEncode(Ext.getCmp('pb_image_link').getValue());
                    var begin_time = Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('pb_startdate').getValue()), 'Y-m-d H:i:s'));
                    var end_time = Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('pb_enddate').getValue()), 'Y-m-d H:i:s'));
                    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "loding..." });
                    myMask.show();
                    form.submit({
                        params: {
                            vb_ids: InsertValues,
                            pb_id: pb_id,
                            image: image,
                            image_link: image_link,
                            begin_time: begin_time,
                            end_time: end_time,
                            multi: multi
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            myMask.hide();
                            if (result.success) {
                                if (result.msg != undefined) {
                                    Ext.Msg.alert(INFORMATION, result.msg);
                                }
                                else {
                                    Ext.Msg.alert(INFORMATION, SUCCESS);
                                }
                                EventPromoImageListStore.load();
                                editWin.close();
                            }
                            else if (result.msg > 1) {
                                Ext.Msg.alert(INFORMATION, "品牌編號 : " + result.msg + " 在指定的時間段內正在使用其他促銷圖片, 請確認");
                                Ext.getCmp('pb_image').setRawValue(image);
                                return;
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                                Ext.getCmp('pb_image').setRawValue(image);
                            }
                        },
                        failure: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            myMask.hide();
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, SUCCESS);
                                EventPromoImageListStore.load();
                                editWin.close();
                            }
                            else if (result.msg > 1) {
                                Ext.Msg.alert(INFORMATION, "品牌編號 : " + result.msg + " 在指定的時間段內正在使用其他促銷圖片, 請確認");
                                Ext.getCmp('pb_image').setRawValue(image);
                                return;
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                                Ext.getCmp('pb_image').setRawValue(image);
                            }
                        }
                    });
                }
            }
        }]
    });

    var editWin = Ext.create('Ext.window.Window', {
        title: '促銷品牌',
        id: 'editWin',
        iconCls: 'icon-user-edit',
        width: 420,
        layout: 'fit',
        items: [frm],
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        constrain: true,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
        {
            type: 'close',
            qtip: '是否關閉',
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
        ],
        listeners: {
            'show': function () {
                if (row) {
                    frm.getForm().loadRecord(row);
                    pbBrandStore.loadPage(1);
                    initForm(row);
                    Ext.getCmp("save").setDisabled(false);
                }
                else {
                    frm.getForm().reset();
                }
            }
        }
    });
    editWin.show();
    function addBrd() {
        brand_name = ""
        add = false;
        var brand_id = Ext.getCmp('b_id').getValue().trim();
        if (!Ext.getCmp('b_id').regex.test(brand_id)) {
            Ext.Msg.alert(INFORMATION, "請輸入正整數格式的品牌編號");
            return;
        }
        else {
            for (var i = 0; i < pbBrandStore.data.length; i++) {
                if (brand_id == pbBrandStore.data.items[i].data.brand_id) {
                    Ext.Msg.alert(INFORMATION, "該品牌編號已添加，請確認");
                    return;
                }
            }
            var bids = del_ids.split('|');
            var bnames = del_names.split('|')
            for (var a = 0; a < bids.length; a++) {
                if (brand_id == bids[a]) {
                    pbBrandStore.add({
                        brand_id: brand_id,
                        brand_name: bnames[a]
                    });
                    Ext.getCmp('b_id').reset();
                    return;
                }
            }
            GetBrandName();
            if (add) {
                pbBrandStore.add({
                    brand_id: brand_id,
                    brand_name: brand_name
                });
                Ext.getCmp('b_id').reset();
            }
        }
    }
}
function delBrand(id) {
    del_ids += id = "|";
    var brands = Ext.getCmp("brand_Grid").store.data.items;
    for (var i = 0; i < brand_con.length; i++) {
        var vb_id = brand_con[i].get("brand_id");
        storeValues += vb_id + ',';
    }
}
function initForm(row) {
    Ext.getCmp("pb_image").setRawValue(row.data.pb_image);
    Ext.getCmp("pb_image_link").setRawValue(row.data.pb_image_link);
    Ext.getCmp("pb_startdate").setRawValue(row.data.pb_startdate);
    Ext.getCmp("pb_enddate").setRawValue(row.data.pb_enddate);
}
function Tomorrow() {
    var d;
    var s = "";
    d = new Date();                             // 创建 Date 对象。
    s += d.getFullYear() + "/";                     // 获取年份。
    s += (d.getMonth() + 1) + "/";              // 获取月份。
    s += (d.getDate() + 1);                          // 获取日。
    return (new Date(s));                                 // 返回日期。
};
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
GetBrandName = function () {
    var brand_id = Ext.getCmp('b_id').getValue();
    Ext.Ajax.request({
        url: '/EventPromo/GetBrandName',
        method: 'post',
        async: false,
        params: {
            pb_id: Ext.getCmp("pb_id").getValue(),
            brand_id: brand_id,
            startdate: Ext.getCmp("pb_startdate").getValue(),
            enddate: Ext.getCmp("pb_enddate").getValue(),
            multi: multi
        },
        success: function (form, action) {
            var result = Ext.decode(form.responseText);
            if (result.success) {
                brand_name = result.msg;
                add = true;
            }
            else if (result.msg == '-1') {
                Ext.Msg.alert(INFORMATION, "該品牌編號不存在，請確認");
                return;
            }
            else if (result.msg == '-2') {
                Ext.Msg.alert(INFORMATION, "品牌編號 : " + Ext.getCmp('b_id').getValue() + " 在指定的時間段內正在使用其他促銷圖片, 請確認");
                return;
            }
            else {
                Ext.Msg.alert(INFORMATION, FAILURE);
                return;
            }
        },
        failure: function () {
            Ext.Msg.alert(INFORMATION, FAILURE);
            return;
        }
    });
}
