
function editFunction(row, store) {
    var currentPanel = 1;
    var navigate = function (panel, direction) {

        var layout = panel.getLayout();
        var move = Ext.getCmp('move-next').text;

        if ('next' == direction) {
            //在這裡判斷料號是否存在
            Ext.Ajax.request({
                url: "/WareHouse/GetLoc_id",//獲取料位編號,判斷是否重複
                method: 'post',
                type: 'text',
                params: {
                    id: Ext.getCmp('loc_id').getValue(),
                    row_id: Ext.getCmp('row_id').getValue()
                },
                success: function (form, action) {
                    var result = Ext.decode(form.responseText);
                    if (result.success) {
                        if (result.data <= 0) {
                            currentPanel++;
                            if (currentPanel == 2) {
                                if (Ext.htmlEncode(Ext.getCmp("lcat_id").getValue().lcat_Type) == "S") {
                                    Ext.getCmp('sel_stk_pos').show();
                                    Ext.getCmp('sel_pos_hgt').show();
                                    Ext.getCmp('rsv_stk_pos').hide();
                                    Ext.getCmp('rsv_pos_hgt').hide();
                                    Ext.getCmp('sel_stk_pos').allowBlank = false;
                                    Ext.getCmp('sel_pos_hgt').allowBlank = false;
                                }
                                else if (Ext.htmlEncode(Ext.getCmp("lcat_id").getValue().lcat_Type) == "R") {
                                    Ext.getCmp('sel_stk_pos').hide();
                                    Ext.getCmp('sel_pos_hgt').hide();
                                    Ext.getCmp('rsv_stk_pos').show();
                                    Ext.getCmp('rsv_pos_hgt').show();
                                    Ext.getCmp('rsv_stk_pos').allowBlank = false;
                                    Ext.getCmp('rsv_pos_hgt').allowBlank = false;
                                }
                                Ext.getCmp('move-prev').setDisabled(false);
                                Ext.getCmp('move-next').hide();
                            }
                            layout[direction]();
                        }
                        else {
                            Ext.MessageBox.alert(INFORMATION, "此料號已存在!請重新添加料號!");
                        }
                    }
                    else {
                        Ext.MessageBox.alert("保存失敗!");
                    }
                }
            });
        }
        else {
            Ext.getCmp('move-next').show();
            currentPanel = 1;
            if (currentPanel == 1) {
                Ext.getCmp('move-prev').setDisabled(true);
                Ext.getCmp('move-next').setText('下一步');
                layout[direction]();
            }
        }
    };
    //料位模式
    var IloctypeStore = Ext.create('Ext.data.Store', {
        fields: ['ides_id', 'ides_name'],
        data: [
            { ides_id: 'RK', ides_name: "標準的鐵架整板料位" },
            { ides_id: 'SF', ides_name: "分割的鐵架料位" },
            { ides_id: 'R', ides_name: "地板料位" },
            { ides_id: 'FW', ides_name: '流利架料位' }
        ]
    });
    var levStore = Ext.create('Ext.data.Store', {
        fields: ['lev_id', 'lev_name'],
        data: [
            { lev_id: '1', lev_name: "A" },
            { lev_id: '2', lev_name: "B" },
            { lev_id: '3', lev_name: "C" },
            { lev_id: '4', lev_name: "D" },
            { lev_id: '5', lev_name: "E" },
            { lev_id: '6', lev_name: "F" },
            { lev_id: '7', lev_name: "G" },
            { lev_id: '8', lev_name: "H" },
            { lev_id: '9', lev_name: "I" }
        ]
    });
    var ldspStore = Ext.create('Ext.data.Store', {
        fields: ['ldsp', 'ldsp_name'],
        data: [
            { ldsp: 'F', ldsp_name: "F" },
            { ldsp: 'D', ldsp_name: "D" }
        ]
    });
    var firstForm = Ext.widget('form', {
        id: 'editFrm1',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
             {
                 xtype: 'textfield',
                 fieldLabel: 'Id',
                 id: 'row_id',
                 name: 'row_id',
                 hidden: true
             },
            {
                fieldLabel: '物流中心編號',
                xtype: 'textfield',
                padding: '10 0 5 0',
                id: 'dc_id',
                name: 'dc_id',
                allowBlank: false,
                labelWidth: 80,
                value: 1,
                hidden:true
            },
            {
                fieldLabel: '倉庫編號',
                xtype: 'textfield',
                padding: '10 0 5 0',
                id: 'whse_id',
                name: 'whse_id',
                allowBlank: false,
                labelWidth: 80,
                value: 1,
                hidden: true
            },
            {
                fieldLabel: '料位編號',
                xtype: 'textfield',
                padding: '10 0 5 0',
                //regex: /^[A-Z]{2}\d{3}[A-Z]\d{2}$/,
                regex: /^[A-Za-z]{2}\d{3}[A-Za-z]\d{2}$/,
                regexText: "料位不規則",
                id: 'loc_id',
                name: 'loc_id',
                allowBlank: false,
                labelWidth: 80
            },
            {
                xtype: 'radiogroup',
                id: 'llts_id',
                name: 'llts_id',
                fieldLabel: "進出方式",
                colName: 'llts_id',
                width: 150,
                defaults: {
                    name: 'llts_Type'
                },
                columns: 2,
                vertical: true,
                items: [
                    { id: 'id1', boxLabel: "先進先出", inputValue: 'F', checked: true },
                    { id: 'id2', boxLabel: "后進先出", inputValue: 'L' }
                ]
            },
              {
                  xtype: 'radiogroup',
                  id: 'lcat_id',
                  name: 'lcat_id',
                  fieldLabel: "是否主料位",
                  colName: 'lcat_id',
                  width: 150,
                  defaults: {
                      name: 'lcat_Type'
                  },
                  columns: 2,
                  vertical: true,
                  items: [
                      { id: 'id7', boxLabel: "主料位", inputValue: 'S', checked: true },
                      { id: 'id8', boxLabel: "副料位", inputValue: 'R' }
                  ]
              },
            {
                xtype: 'combobox',
                fieldLabel: '料位模式',
                id: 'ldes_id',
                queryMode: 'local',
                name: 'ldes_id',
                allowBlank: true,
                editable: false,
                store: IloctypeStore,
                displayField: 'ides_name',
                valueField: 'ides_id',
                emptyText: "請選擇",
                hidden:true
            }
        ]
    });
    //第二步
    var secondForm = Ext.widget('form', {
        id: 'editFrm',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        url: '/WareHouse/InestIloc',
        buttons: [{
            text: "保存",
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                var lociid = Ext.htmlEncode(Ext.getCmp('loc_id').getValue().toUpperCase());
                var regZhengze = /^[A-Z]{2}\d{3}[A-Z]\d{2}$/;
                if (!lociid.match(regZhengze)) {
                    Ext.Msg.alert("提示","料位條碼填寫有誤");
                    return false;
                }
                if (form.isValid()) {
                    form.submit({
                        params: {
                            row_id: Ext.htmlEncode(Ext.getCmp('row_id').getValue()),
                            dc_id: Ext.htmlEncode(Ext.getCmp('dc_id').getValue()),
                            whse_id: Ext.htmlEncode(Ext.getCmp('whse_id').getValue()),
                            loc_id: lociid,
                            llts_id: Ext.htmlEncode(Ext.getCmp("llts_id").getValue().llts_Type),
                            ldes_id: Ext.htmlEncode(Ext.getCmp('ldes_id').getValue()),
                            sel_stk_pos: Ext.htmlEncode(Ext.getCmp('sel_stk_pos').getValue()),
                            sel_pos_hgt: Ext.htmlEncode(Ext.getCmp('sel_pos_hgt').getValue()),
                            rsv_stk_pos: Ext.htmlEncode(Ext.getCmp('rsv_stk_pos').getValue()),
                            rsv_pos_hgt: Ext.htmlEncode(Ext.getCmp('rsv_pos_hgt').getValue()),
                            stk_lmt: Ext.htmlEncode(Ext.getCmp("stk_lmt").getValue().Sktlmt_type),
                            stk_pos_wid: Ext.htmlEncode(Ext.getCmp('stk_pos_wid').getValue()),
                            lev: Ext.htmlEncode(Ext.getCmp('lev').getValue()),
                            lhnd_id: Ext.htmlEncode(Ext.getCmp('lhnd_id').getValue()),
                            lcat_id: Ext.htmlEncode(Ext.getCmp("lcat_id").getValue().lcat_Type),
                            stk_pos_dep: Ext.htmlEncode(Ext.getCmp('stk_pos_dep').getValue()),
                            comingle_allow: Ext.htmlEncode(Ext.getCmp("comingle_allow").getValue().Comingle_type)
                            //,
                            //ldsp_id:Ext.htmlEncode(Ext.getCmp('ldsp_id').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, "保存成功！");
                                IlocsStore.load();
                                editWin.close();
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, PROMODISCOUNTTIP);
                            }
                        },
                        failure: function (form, action) {
                            Ext.Msg.alert(INFORMATION, "保存失敗");
                        }
                    });
                }
            }
        }],
        //url: '/PromotionsBonus/Save',
        defaults: { anchor: "90%", msgTarget: "side" },
        items: [
            {
                xtype: 'numberfield',
                fieldLabel: "主料位置貨空間",
                name: 'sel_stk_pos',
                id: 'sel_stk_pos',
                hidden: true,
                allowBlank: true,
                minValue:0
            },
            {
                xtype: 'numberfield',
                fieldLabel: "主料位空間高度",
                name: 'sel_pos_hgt',
                id: 'sel_pos_hgt',
                hidden: true,
                allowBlank: true,
                minValue: 0

            },
            {
                xtype: 'numberfield',
                fieldLabel: "副料位疊貨空間",
                name: 'rsv_stk_pos',
                id: 'rsv_stk_pos',
                hidden: true,
                allowBlank: true,
                minValue: 0
            },
            {
                xtype: 'numberfield',
                name: 'rsv_pos_hgt',
                id: 'rsv_pos_hgt',
                fieldLabel: "副料位疊貨高度",
                hidden: true,
                allowBlank: true,
                minValue: 0

            },
            {
                xtype: 'radiogroup',
                id: 'stk_lmt',
                name: 'stk_lmt',
                fieldLabel: "是否可放多個棧板",
                colName: 'stk_lmt',
                width: 150,
                defaults: {
                    name: 'Sktlmt_type'
                },
                columns: 2,
                vertical: true,
                items: [
                    { id: 'id3', boxLabel: "是", inputValue: '1', checked: true },
                    { id: 'id4', boxLabel: "否", inputValue: '2' }
                ]
            },
             {
                 xtype: 'radiogroup',
                 id: 'comingle_allow',
                 name: 'comingle_allow',
                 fieldLabel: "是否啟用多板功能",
                 colName: 'comingle_allow',
                 width: 150,
                 defaults: {
                     name: 'Comingle_type'
                 },
                 columns: 2,
                 vertical: true,
                 items: [
                     { id: 'id5', boxLabel: "是", inputValue: 'Y' },
                     { id: 'id6', boxLabel: "否", inputValue: 'N', checked: true }
                 ]
             },
            {
                fieldLabel: '料位寬度',
                xtype: 'numberfield',
                padding: '10 0 5 0',
                id: 'stk_pos_wid',
                name: 'stk_pos_wid',
                allowBlank: false,
                minValue:0
            },
             {
                 fieldLabel: '料位深度',
                 xtype: 'numberfield',
                 padding: '10 0 5 0',
                 id: 'stk_pos_dep',
                 name: 'stk_pos_dep',
                 allowBlank: false,
                 minValue: 0
             },
            {
                xtype: 'combobox',
                fieldLabel: '所在層數',
                id: 'lev',
                queryMode: 'local',
                name: 'lev',
                allowBlank: false,
                editable: false,
                typeAhead: true,
                forceSelection: false,
                store: levStore,
                displayField: 'lev_name',
                valueField: 'lev_id',
                emptyText: "請選擇"
            },
            {
                fieldLabel: '撿貨單位',
                xtype: 'textfield',
                id: 'lhnd_id',
                name: 'lhnd_id',
                allowBlank: false
            },
              {
                  xtype: 'combobox',
                  fieldLabel: '',
                  id: 'ldsp_id',
                  queryMode: 'local',
                  name: 'ldsp_id',
                  allowBlank:true,
                  editable: false,
                  typeAhead: true,
                  forceSelection: false,
                  store: ldspStore,
                  displayField: 'ldsp_name',
                  valueField: 'ldsp',
                  emptyText: "請選擇",
                  hidden:true
              } //該項已經取消
        ]
    });
    var allpan = new Ext.panel.Panel({
        width: 390,
        layout: 'card',
        border: 0,
        bodyStyle: 'padding:0px',
        defaults: { // 应用到所有子面板           
            border: false
        },
        // 这里仅仅用几个按钮来示例一种可能的导航场景.
        bbar: [
        {
            id: 'move-prev',
            text: '上一步',
            handler: function (btn) {
                navigate(btn.up("panel"), "prev");
            },
            disabled: true
        },
        '->', // 一个长间隔, 使两个按钮分布在两边
        {
            id: 'move-next',
            text: '下一步',
            handler: function (btn) {
                navigate(btn.up("panel"), "next");
            }
        }
        ],

        // 布局下的各子面板 //firstForm,secondForm
        items: [firstForm, secondForm]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: "料位",
        iconCls: 'icon-user-edit',
        width: 400,
        y: 100,
        layout: 'fit',
        items: [allpan],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        closable: false,
        tools: [
             {
                 type: 'close',
                 qtip: "關閉",
                 handler: function (event, toolEl, panel) {
                     Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                         if (btn == "yes") {
                             editWin.destroy();
                         }
                         else {
                             return false;
                         }
                     });
                 }
             }],
        listeners: {
            'show': function () {
                if (row != null) {
                    firstForm.getForm().loadRecord(row);
                    secondForm.getForm().loadRecord(row);
                    Ext.getCmp('dc_id').hide();
                    Ext.getCmp('whse_id').hide();
                    Ext.getCmp('loc_id').setDisabled(true);
                    if (row.data.llts_id == "F") {
                        Ext.getCmp('id1').setValue(true);
                        Ext.getCmp('id2').setValue(false);

                    }
                    else if (row.data.llts_id == "L") {
                        Ext.getCmp('id1').setValue(false);
                        Ext.getCmp('id2').setValue(true);
                    }

                    if (row.data.stk_lmt == 1) {
                        Ext.getCmp('id3').setValue(true);
                        Ext.getCmp('id4').setValue(false);

                    }
                    else if (row.data.stk_lmt == 2) {
                        Ext.getCmp('id3').setValue(false);
                        Ext.getCmp('id4').setValue(true);
                    }

                    if (row.data.lcat_id == "S")
                    {
                        Ext.getCmp('id7').setValue(true);
                        Ext.getCmp('id8').setValue(false);
                    }
                    else if (row.data.lcat_id == "R")
                    {
                        Ext.getCmp('id7').setValue(false);
                        Ext.getCmp('id8').setValue(true);
                    }
                    Ext.getCmp('lcat_id').setDisabled(true);
                    if (row.data.comingle_allow == "Y")
                    {
                       
                        Ext.getCmp('id5').setValue(true);
                        Ext.getCmp('id6').setValue(false);
                    }
                    else if (row.data.comingle_allow == "N")
                    {
                    
                        Ext.getCmp('id5').setValue(false);
                        Ext.getCmp('id6').setValue(true);
                    }
                }
            }
        }
    });
    editWin.show();
}
