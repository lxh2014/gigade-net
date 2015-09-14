function editFunction(rowID) {


    var sexStore = Ext.create('Ext.data.Store', {
        fields: ['txt', 'value'],
        data: [
            { "txt": "多於", "value": "1" },
            { "txt": "等於", "value": "2" },
             { "txt": "少於", "value": "3" },
        ]
    });

    Ext.define('gigade.zipAddress', {
        extend: 'Ext.data.Model',
        fields: [
            { name: "zipcode", type: "string" },
            { name: "zipname", type: "string" }
        ]
    });
    var zipStore = Ext.create('Ext.data.Store', {
        model: 'gigade.zipAddress',
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: "/VipUserGroup/GetZipStore",
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });

    Ext.define('gigade.zipAddress2', {
        extend: 'Ext.data.Model',
        fields: [
            { name: "zipcode", type: "string" },
            { name: "zipname", type: "string" }
        ]
    });
    var zipStore2 = Ext.create('Ext.data.Store', {
        model: 'gigade.zipAddress2',
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: "/VipUserGroup/GetZipStore",
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });

    Ext.define('gigade.companyAddress', {
        extend: 'Ext.data.Model',
        fields: [
            { name: "da_id", type: "int" },
            { name: "user_id", type: "int" },
            { name: "da_title", type: "string" },
            { name: "da_dist", type: "string" },
            { name: "da_address", type: "string" },
        ]
    });
    var companyAddressStore = Ext.create('Ext.data.Store', {
        model: 'gigade.companyAddress',
        autoLoad: false,
        proxy: {
            type: 'ajax',
            url: "/VipUserGroup/GetComAddress",
            reader: {
                type: 'json',
                root: 'data'
            }
        },
        listeners: {
            update: function (store, record) {
                if (record.isModified('da_address') || record.isModified('da_dist') || record.isModified('da_title')) {
                    Ext.Ajax.request({
                        url: '/VipUserGroup/SaveComAddress',
                        params: {
                            da_id: record.get("da_id"),
                            da_title: record.get("da_title"),
                            da_address: record.get("da_address"),
                            da_dist: record.get("da_dist"),
                        },
                        success: function (form, action) {
                            var result = Ext.decode(form.responseText);
                            if (result.success) {
                                companyAddressStore.load({ params: { user_id: Ext.getCmp('group_id').getValue() } });
                            }
                            else {
                                Ext.Msg.alert("提示信息", "保存失敗！");
                            }
                        },
                        failure: function () {
                            Ext.Msg.alert("提示信息", "保存失敗！");
                        }
                    });
                }
            }
        }
    });
    Ext.define('gigade.GroupCommitteContact', {
        extend: 'Ext.data.Model',
        fields: [
            { name: "gcc_id", type: "int" },
            { name: "group_id", type: "int" },
            { name: "gcc_chairman", type: "string" },
            { name: "gcc_phone", type: "string" },
            { name: "gcc_mail", type: "string" }
        ]
    });
    var gccStore = Ext.create('Ext.data.Store', {
        model: 'gigade.GroupCommitteContact',
        autoLoad: false,
        proxy: {
            type: 'ajax',
            url: "/VipUserGroup/GetGroupCommitteContact",
            reader: {
                type: 'json',
                root: 'data'
            }
        },
        listeners: {
            update: function (store, record) {
                if (record.isModified('gcc_chairman') || record.isModified('gcc_phone') || record.isModified('gcc_mail')) {
                    Ext.Ajax.request({
                        url: '/VipUserGroup/UpdateGCC',
                        params: {
                            gcc_id: record.get("gcc_id"),
                            gcc_chairman: record.get("gcc_chairman"),
                            gcc_phone: record.get("gcc_phone"),
                            gcc_mail: record.get("gcc_mail"),
                        },
                        success: function (form, action) {
                            var result = Ext.decode(form.responseText);
                            if (result.success) {
                                gccStore.load({ params: { group_id: Ext.getCmp('group_id').getValue() } });
                            }
                            else {
                                Ext.Msg.alert("提示信息", "保存失敗！");
                            }
                        },
                        failure: function () {
                            Ext.Msg.alert("提示信息", "保存失敗！");
                        }
                    });
                }
            }
        }
    });


    var currentPanel = 0;
    var navigate = function (panel, direction) {
        var layout = panel.getLayout();
        var store = gccStore.data.items;
        if (store.length != 0)
        {
            Ext.getCmp('group_committe_chairman').allowBlank = true;
            Ext.getCmp('group_committe_phone').allowBlank = true;
            Ext.getCmp('group_committe_mail').allowBlank = true;
        }
        //如果點的是下一步
        if ('next' == direction) {
            var firForm = firstForm.getForm();
            var ageChecked = Ext.getCmp("group_emp_age").items;
            var ageCheck = "";
            for (var i = 0; i < ageChecked.length; i++) {
                if (ageChecked.get(i).checked) {
                    ageCheck = ageCheck + ageChecked.get(i).inputValue + ",";
                }
            }
            var benefitChecked = Ext.getCmp("group_benefit_type").items;
            var benefitCheck = "";
            for (var i = 0; i < benefitChecked.length; i++) {
                if (benefitChecked.get(i).checked) {
                    benefitCheck = benefitCheck + benefitChecked.get(i).inputValue + ",";
                }
            }
            //第一個面板
            if (currentPanel == 0) {
                Ext.getCmp('move-next').setDisabled(!layout.getNext());
                var firForm = firstForm.getForm();
                //add
                if (!row) {
                    if (Ext.getCmp('group_id').getValue() == "") {
                        if (firForm.isValid()) {
                            firForm.submit({
                                url: '/VipUserGroup/InsertVipUserGroup',
                                params: {
                                    group_name: Ext.getCmp('group_name').getValue(),
                                    eng_name: Ext.getCmp('eng_name').getValue(),
                                    tax_id: Ext.getCmp('tax_id').getValue(),
                                    group_code: Ext.getCmp('group_code').getValue(),
                                    check_iden: Ext.getCmp('check_iden').getValue().c_id,
                                    group_capital: Ext.getCmp('group_capital').getValue(),
                                    group_emp_number: Ext.getCmp('group_emp_number').getValue(),
                                    group_emp_age: ageCheck,
                                    group_emp_gender: Ext.getCmp('group_emp_gender').getValue(),
                                    group_benefit_type: benefitCheck,
                                    group_benefit_desc: Ext.getCmp('group_benefit_desc').getValue(),
                                    group_subsidiary: Ext.getCmp('group_subsidiary').getValue().gsu,
                                    group_hq_name: Ext.getCmp('group_hq_name').getValue(),
                                    group_hq_code: Ext.getCmp('group_hq_code').getValue(),
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(action.response.responseText);
                                    if (result.success) {
                                        group_id = result.msg;
                                        Ext.getCmp('group_id').setValue(result.msg);
                                        layout[direction]();
                                        currentPanel++;
                                        Ext.getCmp('move-prev').show();

                                    }
                                }

                            });
                        }
                    }
                    else {
                        if (firForm.isValid()) {
                            layout[direction]();
                            currentPanel++;
                            Ext.getCmp('move-prev').show();
                        }
                    }
                }
                    //EDIT
                else {
                    if (firForm.isValid()) {
                        layout[direction]();
                        currentPanel++;
                        Ext.getCmp('move-prev').show();
                        //if (row.data.file_name) {
                        //    Ext.getCmp('saveForth').setDisabled(false);
                        //}
                        Ext.getCmp('group_id').setValue(row.data.group_id);
                        Ext.getCmp('group_committe_name').setValue(row.data.group_committe_name);
                        Ext.getCmp('group_committe_code').setValue(row.data.group_committe_code);
                        //Ext.getCmp('group_committe_chairman').setValue(row.data.group_committe_chairman);
                        //Ext.getCmp('group_committe_phone').setValue(row.data.group_committe_phone);
                        //Ext.getCmp('group_committe_mail').setValue(row.data.group_committe_mail);
                        Ext.getCmp('old_group_committe_mail').setValue(row.data.group_committe_mail);
                        Ext.getCmp('employee_list').setRawValue(row.data.file_name);
                        var promotion = row.data.group_committe_promotion;
                        var promotionArr = promotion.split(',');
                        for (var i = 0; i < promotionArr.length; i++) {
                            switch (promotionArr[i]) {
                                case "1":
                                    Ext.getCmp('pro_shop').setValue(true);
                                    break;
                                case "2":
                                    Ext.getCmp('pro_templ').setValue(true);
                                    break;
                                case "0":
                                    Ext.getCmp('pro_other').setValue(true);
                                    Ext.getCmp('group_committe_other').setValue(promotionArr[promotionArr.length - 1]);

                                    break;
                            }
                        }
                        Ext.getCmp('group_committe_desc').setValue(row.data.group_committe_desc);
                        companyAddressStore.load({ params: { user_id: Ext.getCmp('group_id').getValue() } });
                        gccStore.load({ params: { group_id: Ext.getCmp('group_id').getValue() } });
                    }
                }
            }
                //進入了下一步
            else if (currentPanel == 1) {
                var secForm = secondForm.getForm();
                if (secForm.isValid()) {
                    layout[direction]();
                    currentPanel++;
                    Ext.getCmp('move-prev').show();
                    Ext.getCmp('move-next').hide();
                }
                else {                  
                    var chair = (Ext.getCmp('group_committe_chairman').allowBlank);
                    var phone = (Ext.getCmp('group_committe_chairman').allowBlank);
                    var mail = (Ext.getCmp('group_committe_chairman').allowBlank);
                    var shop = (Ext.getCmp('pro_shop').getValue());
                    var templ = (Ext.getCmp('pro_templ').getValue());
                    var other = (Ext.getCmp('pro_other').getValue());
                    if (chair && phone  && mail) {
                        if (other || templ || shop) {
                            if (other) {
                                if (Ext.getCmp('group_committe_other').getValue() == "") {
                                    return;
                                } else {
                                    layout[direction]();
                                    currentPanel++;
                                    Ext.getCmp('move-prev').show();
                                    Ext.getCmp('move-next').hide();
                                }
                            }
                            else {
                                layout[direction]();
                                currentPanel++;
                                Ext.getCmp('move-prev').show();
                                Ext.getCmp('move-next').hide();
                            }
                        }
                    }
                }
            }
                //如果是最後一個面板，下一步隱藏
            else if (currentPanel == 2) {
                Ext.getCmp('move-next').hide();

            }
        }
        else {
            layout[direction]();
            currentPanel--;
            Ext.getCmp('move-prev').show();
            Ext.getCmp('move-next').show();
            //如果是第一個面板，上一步隱藏
            if (currentPanel == 0) {
                Ext.getCmp('move-prev').hide();
            }
        }
    }

    var firstForm = Ext.widget('form', {
        id: 'firstForm',
        plain: true,
        frame: true,
        height: 560,
        layout: 'anchor',
        //  defaults: { anchor: "95%", msgTarget: "side" },
        items: [
             {
                 xtype: 'displayfield',
                 value: '<span style="font-size:15px; color:black;">公司基本資料</span>',
             },

            {
                xtype: 'textfield',
                fieldLabel: '中文名稱',
                name: 'group_name',
                id: 'group_name',
                allowBlank: false,
                maxLength: 255,
                width: 490,
            },
             {
                 xtype: 'textfield',
                 fieldLabel: '英文名稱',
                 name: 'eng_name',
                 id: 'eng_name',
                 allowBlank: false,
                 maxLength: 50,
                 width: 490,
             },
             {
                 xtype: 'textfield',
                 fieldLabel: '公司統編',
                 id: 'old_group_code',
                 name: 'old_group_code',
                 hidden: true,
                 maxLength: 8,
             
                 width: 255,
             },
             {
                 xtype: 'textfield',
                 fieldLabel: '公司統編',
                 id: 'group_code',
                 name: 'group_code',
                 allowBlank: false,
                 maxLength: 8,
                 minLength: 8,
                 width: 255,
                 listeners: {
                     blur: function () {
                         var oldgroup_code = Ext.getCmp('old_group_code').getValue();
                         var group_code = Ext.getCmp('group_code').getValue();
                         if (oldgroup_code != group_code) {
                             if (group_code != "") {
                                 Ext.Ajax.request({
                                     url: '/VipUserGroup/ExisGroupCode',
                                     params: {
                                         //group_id: Ext.getCmp('group_id').getValue(),
                                         group_code: Ext.getCmp('group_code').getValue(),
                                     },
                                     success: function (form, action) {
                                         var result = Ext.decode(form.responseText);
                                         if (result.success) {
                                             Ext.Msg.alert("提示信息", "此公司統編已存在，請重新填寫！");
                                             Ext.getCmp('group_code').setValue("");
                                         }
                                     }
                                 });
                             }

                         }

                     }
                 },
             },
          {
              xtype: 'textfield',
              fieldLabel: '公司代號',
              id: 'tax_id',
              name: 'tax_id',
              minLength: 8,
              maxLength: 8,
              allowBlank: false,
              width: 255,
              listeners: {
                  blur: function () {
                      var tax_id = Ext.getCmp('tax_id').getValue();
                          if (tax_id != "") {
                              Ext.Ajax.request({
                                  url: '/VipUserGroup/ExisTaxId',
                                  params: {
                                      group_id: Ext.getCmp('group_id').getValue(),
                                      tax_id: tax_id,
                                  },
                                  success: function (form, action) {
                                      var result = Ext.decode(form.responseText);
                                      if (result.success) {
                                          Ext.Msg.alert("提示信息", "此公司代號已存在，請重新填寫！");
                                          Ext.getCmp('tax_id').setValue("");
                                      }
                                  }
                              });
                          }

                  }
              }
          },
          {
              xtype: 'radiogroup',
              id: 'check_iden',
              name: 'check_iden',
              allowBlank: false,
              width: 260,
              fieldLabel: '員工編號驗證',
              items: [
                  { id: 'yes', name: 'c_id', boxLabel: '是', inputValue: 1,  },
                  {
                      id: 'no', name: 'c_id', boxLabel: '否', inputValue: 0, checked: true
                  },
              ],
              //listeners: {
              //    change: function () {
              //        var check = Ext.getCmp('check_iden').getValue().c_id;
              //        if (check == 1)
              //        {
              //            Ext.getCmp('employee_list').allowBlank=true;
              //        }
              //        if (check == 0) {
              //            Ext.getCmp('employee_list').allowBlank = false;
              //        }
              //    }
              //},
          },
          {
              xtype: 'fieldcontainer',
              layout: 'hbox',
              items: [
                        {
                            xtype: 'numberfield',
                            fieldLabel: '資本額',
                            id: 'group_capital',
                            name: 'group_capital',
                            minValue: 0,
                            allowDecimals: false,
                            maxValue: 2147483647,
                        },
                        {
                            xtype: 'displayfield',
                            value: '元',
                        },
              ]
          },
              {
                  xtype: 'fieldcontainer',
                  layout: 'hbox',
                  items: [
                             {
                                 xtype: 'numberfield',
                                 fieldLabel: '員工總人數',
                                 id: 'group_emp_number',
                                 name: 'group_emp_number',
                                 minValue: 0,
                                 allowDecimals: false,
                                 maxValue: 2147483647,
                             },
                            {
                                xtype: 'displayfield',
                                value: '位',
                            },
                  ]
              },

         {
             xtype: 'checkboxgroup',
             fieldLabel: '年齡分布',
             columns: 3,
             id: 'group_emp_age',
             name: 'group_emp_age',
             //    vertical: true,
             allowBlank: false,
             items: [
                 { boxLabel: '20歲(含)以下', id: '20', inputValue: '1' },
                 { boxLabel: '21~30歲', id: '21_30', inputValue: '2' },
                 { boxLabel: '31~40歲', id: '31_40', inputValue: '3' },
                 { boxLabel: '41~50歲', id: '41_50', inputValue: '4' },
                 { boxLabel: '51~60歲', id: '51_60', inputValue: '5' },
                 { boxLabel: '61歲以上', id: '61', inputValue: '6' }
             ]

         },
         {
             xtype: 'fieldcontainer',
             layout: 'hbox',
             fieldLabel: '男女比率',
             items: [
                 {
                     xtype: 'displayfield',
                     value: '男生',
                 },
                     {
                         xtype: 'combobox',
                         editable: false,
                         store: sexStore,
                         displayField: 'txt',
                         valueField: 'value',
                         id: 'group_emp_gender',
                         name: 'group_emp_gender',
                         value: '1'
                     },
                       {
                           xtype: 'displayfield',
                           value: '女生',
                       },
             ],
         },

                  {
                      xtype: 'checkboxgroup',
                      fieldLabel: '福利類別',
                      columns: 3,
                      id: 'group_benefit_type',
                      name: 'group_benefit_type',
                      //  vertical: true,
                      allowBlank: false,
                      items: [
                          { boxLabel: '福利金', id: 'welfare', inputValue: '1' },
                          { boxLabel: '年節禮券', id: 'gift_cash', inputValue: '2' },
                          { boxLabel: '年節禮盒', id: 'gift_box', inputValue: '3' },
                          { boxLabel: '現金', id: 'cash', inputValue: '4' },
                          { boxLabel: '旅遊津貼', id: 'tour', inputValue: '5' },
                          { boxLabel: '其他', id: 'other', inputValue: '6' }
                      ]

                  },
                  {
                      xtype: 'textarea',
                      fieldLabel: '福利發放簡述(最多200字)',
                      id: 'group_benefit_desc',
                      name: 'group_benefit_desc',
                      maxLength: 200,
                      width: 490,
                  },
                  {
                      xtype: 'fieldset',
                      items: [
                            {
                                xtype: 'radiogroup',
                                id: 'group_subsidiary',
                                name: 'group_subsidiary',
                                fieldLabel: '是否子公司',
                                width: 260,
                                allowBlank: false,
                                items: [
                                    { boxLabel: '是', name: 'gsu', id: 'gs1', inputValue: '1' },
                                    { boxLabel: '否', name: 'gsu', id: 'gs0', inputValue: '0' },
                                ],
                                listeners: {
                                    change: function () {
                                        var value = Ext.getCmp('group_subsidiary').getValue().gsu;
                                        if (value == 1) {
                                            Ext.getCmp('group_hq_name').allowBlank = false;
                                            Ext.getCmp('group_hq_code').allowBlank = false;
                                            Ext.getCmp('group_hq_code').setDisabled(false);
                                            Ext.getCmp('group_hq_name').setDisabled(false);
                                        }
                                        else {
                                            Ext.getCmp('group_hq_code').allowBlank = true;
                                            Ext.getCmp('group_hq_name').allowBlank = true;
                                            Ext.getCmp('group_hq_code').setValue("");
                                            Ext.getCmp('group_hq_name').setValue("");
                                            Ext.getCmp('group_hq_code').setDisabled(true);
                                            Ext.getCmp('group_hq_name').setDisabled(true);

                                        }
                                    }
                                }
                            },
                          {
                              xtype: 'fieldcontainer',
                              layout: 'hbox',
                              items: [
                                  {
                                      xtype: 'displayfield',
                                      margin: '0 0 0 18',
                                      value: '此為'
                                  },
                                  {
                                      xtype: 'textfield',
                                      id: 'group_hq_name',
                                      name: 'group_hq_name',
                                      maxLength: 50,
                                  },
                                      {
                                          xtype: 'displayfield',
                                          value: '旗下之子公司'
                                      },


                              ]
                          },
                          {
                              xtype: 'fieldcontainer',
                              layout: 'hbox',
                              items: [
                         {
                             xtype: 'displayfield',
                             value: '總公司統編'
                         },
                          {
                              xtype: 'textfield',
                              id: 'group_hq_code',
                              name: 'group_hq_code',
                              maxLength: 8,
                              minLength: 8,
                          },
                              ]
                          },

                      ],
                  },
        ]
    });
    var rowEditing = Ext.create('Ext.grid.plugin.RowEditing', {
        clicksToMoveEditor: 2,
        listeners: {
            beforeedit: function (e, eopts) {
                if (e.colidx == 0) {
                    e.hide();
                }
            }
        },
        autoCancel: false,
        clicksToEdit: 2,
        errorSummary: false
    });
    var rowEditing2 = Ext.create('Ext.grid.plugin.RowEditing', {
        clicksToMoveEditor: 2,
        listeners: {
            beforeedit: function (e, eopts) {
                if (e.colidx == 0) {
                    e.hide();
                }
            }
        },
        autoCancel: false,
        clicksToEdit: 2,
        errorSummary: false
    });
    Ext.grid.RowEditor.prototype.saveBtnText = "保存";
    Ext.grid.RowEditor.prototype.cancelBtnText = "取消";
    var gcc_Grid = Ext.create('Ext.grid.Panel', {
        id: 'gcc_Grid',
        plugins: [rowEditing2],
        store: gccStore,
        height: 120,
        columnLines: true,
        // autoScroll:true,       
        scroll:false, 
        viewConfig: {
            style: { overflow: 'auto', overflowX: 'hidden' }
        },
        frame: true,
        columns: [
           {
               header: '刪除', xtype: 'actioncolumn', flex:1, align: 'center',
               items: [
                   {
                       icon: '../../../Content/img/icons/cross.gif',
                       tooltip: '刪除',
                       handler: function (grid, rowIndex, colIndex) {
                           Ext.Msg.confirm("注意", "確認刪除？", function (btn) {
                               if (btn === 'yes') {
                                   var rec = grid.getStore().getAt(rowIndex);
                                   var gcc_id = rec.get('gcc_id');
                                   var gcc_chairman = rec.get('gcc_chairman');
                                   var gcc_phone = rec.get('gcc_phone');
                                   var gcc_mail = rec.get('gcc_mail');
                                   var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
                                   myMask.show();
                                   Ext.Ajax.request({
                                       url: "/VipUserGroup/DeleteGCC",
                                       method: 'post',
                                       params: {
                                           gcc_id: gcc_id,
                                           gcc_chairman: gcc_chairman,
                                           gcc_phone: gcc_phone,
                                           gcc_mail: gcc_mail
                                       },
                                       success: function (form, action) {
                                           myMask.hide();
                                           var result = Ext.decode(form.responseText);
                                           if (result.success) {
                                               myMask.hide();
                                               Ext.Msg.alert("提示信息", "刪除成功");
                                               gccStore.load({ params: { group_id: Ext.getCmp('group_id').getValue() } });
                                           }
                                       },
                                       failure: function () {
                                           myMask.hide();
                                           Ext.Msg.alert("提示信息", "刪除失敗");
                                       }
                                   });
                               }
                           });
                       }
                   }
               ]
           },
           {
               header: '編號', dataIndex: 'gcc_id', flex:1, align: 'center'
           },

           {
               header: '福委姓名', dataIndex: 'gcc_chairman', flex: 1, align: 'center',
               editor: {
                   xtype: 'textfield',
                   maxLength: 50
               }
           },
           {
               header: '福委電話', dataIndex: 'gcc_phone', flex: 1, align: 'center',
               editor: {
                   xtype: 'textfield',
                   maxLength: 50,
                   regex: /^[-+]?[\d]+$/
               }
           },
           {
               header: '福委email', dataIndex: 'gcc_mail', flex: 2, align: 'center',
               editor: {
                   xtype: 'textfield',
                   maxLength: 50,
                   vtype: 'email'
               }
           },
        ]
    });
    gcc_Grid.store.sort([
        {
            property: 'gcc_id', direction: 'DESC'
        }
    ]);
    var secondForm = Ext.widget('form', {
        id: 'secondForm',
        plain: true,
        height: 595,
        frame: true,
        layout: 'anchor',
        // defaults: { anchor: "95%", msgTarget: "side" },
        items: [
              {
                  xtype: 'displayfield',
                  value: '<span style="font-size:15px; color:black;">公司福委資料</span>',
              },
              {
                  xtype: 'displayfield',
                  id: 'group_id',
                  name: 'group_id',
                  fieldLabel: '公司編號',
                  width: 490,
              },
            {
                xtype: 'textfield',
                fieldLabel: '福委会全名',
                name: 'group_committe_name',
                id: 'group_committe_name',
                maxLength: 50,
                width: 490,
            },
             {
                 xtype: 'textfield',
                 fieldLabel: '福委会統編',
                 name: 'group_committe_code',
                 id: 'group_committe_code',
                 maxLength: 50,
                 width: 490,
             },
              {
                  xtype: 'textfield',
                  fieldLabel: '福委姓名',
                  name: 'group_committe_chairman',
                  id: 'group_committe_chairman',
                  allowBlank: false,
                  maxLength: 50,
                  width: 490,
              },
                 {
                     xtype: 'textfield',
                     fieldLabel: '福委電話',
                     name: 'group_committe_phone',
                     id: 'group_committe_phone',
                     allowBlank: false,
                     regex: /^[-+]?[\d]+$/,
                     width: 490,
                 },
                 {
                     xtype: 'textfield',
                     fieldLabel: 'old福委mail',
                     name: 'old_group_committe_mail',
                     id: 'old_group_committe_mail',
                     allowBlank: false,
                     //vtype: 'email',
                     hidden: true,
                 },
                   {
                       xtype: 'textfield',
                       fieldLabel: '福委mail',
                       name: 'group_committe_mail',
                       id: 'group_committe_mail',
                       allowBlank: false,
                       vtype: 'email',
                       maxLength: 100,
                       width: 490,
                       listeners: {
                           //blur: function () {
                           //    var oldMail = Ext.getCmp('old_group_committe_mail').getValue();
                           //    var mail = Ext.getCmp('group_committe_mail').getValue();
                           //    if (oldMail != mail) {
                           //        if (mail != "") {
                           //            Ext.Ajax.request({
                           //                url: '/VipUserGroup/ExistEmail',
                           //                params: {
                           //                    group_id: Ext.getCmp('group_id').getValue(),
                           //                    group_committe_mail: Ext.getCmp('group_committe_mail').getValue(),
                           //                },
                           //                success: function (form, action) {
                           //                    var result = Ext.decode(form.responseText);
                           //                    if (result.success) {
                           //                        Ext.Msg.alert("提示信息", "此福委信箱已存在，請重新填寫！");
                           //                        Ext.getCmp('group_committe_mail').setValue("");
                           //                    }
                           //                }
                           //            });
                           //        }

                           //    }

                           //}
                       },

                   },
                   {
                       xtype: 'button',
                       text: '新增',
                       margin: '0 10 3 450',
                       handler: addgcc
                   },
                  gcc_Grid,
                     {
                         xtype: 'displayfield',
                         value: '<span style="font-size:15px; color:black;">福利推廣</span>',
                     },
                     {
                         xtype: 'fieldset',
                         height: 120,
                         items: [
                               {
                                   xtype: 'checkboxgroup',
                                   fieldLabel: '福利推廣',
                                   columns: 3,

                                   id: 'group_committe_promotion',
                                   name: 'group_committe_promotion',
                                   vertical: true,
                                   allowBlank: false,
                                   items: [
                                       { boxLabel: '特約商店', id: 'pro_shop', inputValue: '1' },
                                       { boxLabel: '其他', id: 'pro_other', inputValue: '0' },
                                       { boxLabel: '特約購物平臺', id: 'pro_templ', inputValue: '2' },
                                           {
                                               xtype: 'textarea',
                                               id: 'group_committe_other',
                                               name: 'group_committe_other',
                                               maxLength: 90,
                                               width: 256,
                                            //   height: 45,
                                               disabled: true,
                                           },
                                   ],
                                   listeners: {
                                       change: function () {
                                           var value = (Ext.getCmp('pro_other').getValue())
                                           if (value) {
                                               Ext.getCmp('group_committe_other').setDisabled(false);
                                               Ext.getCmp('group_committe_other').allowBlank = false;
                                           }
                                           else {
                                               Ext.getCmp('group_committe_other').setValue("");
                                               Ext.getCmp('group_committe_other').setDisabled(true);
                                               Ext.getCmp('group_committe_other').allowBlank = true;
                                           }
                                       }
                                   }
                               },
                         ]
                     },

                               {
                                   xtype: 'textarea',
                                   fieldLabel: '推廣簡述(最多200字)',
                                   id: 'group_committe_desc',
                                   name: 'group_committe_desc',
                                   maxLength: 200,
                                   width: 490,
                                   height: 75,
                               },
        ]
    });


    var comGrid = Ext.create('Ext.grid.Panel', {
        id: 'comGrid',
        plugins: [rowEditing],
        store: companyAddressStore,
        height: 220,
        columnLines: true,
        frame: true,
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        columns: [
            {
                header: '刪除', xtype: 'actioncolumn', width: 60, align: 'center',
                items: [
                    {
                        icon: '../../../Content/img/icons/cross.gif',
                        tooltip: '刪除',

                        handler: function (grid, rowIndex, colIndex) {
                            Ext.Msg.confirm("注意", "確認刪除？", function (btn) {
                                if (btn === 'yes') {
                                    var rec = grid.getStore().getAt(rowIndex);
                                    var da_id = rec.get('da_id');
                                    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
                                    myMask.show();
                                    Ext.Ajax.request({
                                        url: "/VipUserGroup/DeleteDeliveryAddress",
                                        method: 'post',
                                        params: {
                                            da_id: da_id,
                                        },
                                        success: function (form, action) {
                                            myMask.hide();
                                            var result = Ext.decode(form.responseText);
                                            if (result.success) {
                                                myMask.hide();
                                                Ext.Msg.alert("提示信息", "刪除成功");
                                                companyAddressStore.load({ params: { user_id: Ext.getCmp('group_id').getValue() } });
                                            }
                                        },
                                        failure: function () {
                                            myMask.hide();
                                            Ext.Msg.alert("提示信息", "刪除失敗");
                                        }
                                    });
                                }
                            });
                        }
                    }
                ]
            },
            {
                header: '編號', dataIndex: 'da_id', width: 90, align: 'center'
            },
            {
                header: '配送名稱', dataIndex: 'da_title', width: 150, align: 'center',
                editor: {
                    xtype: 'textfield',
                    maxLength: 50,
                }
            },
            {
                header: '配送地址', dataIndex: 'da_dist', width: 150, align: 'center',
                editor: {
                    xtype: 'combobox',
                    id: 'com_address_grid',
                    queryModel: 'local',
                    store: zipStore,
                    valueField: 'zipcode',
                    editable: false,
                    displayField: 'zipname',
                    queryModel: 'local',
                }

            },
              {
                  header: '地址', dataIndex: 'da_address', width: 180, align: 'center',
                  editor: {
                      xtype: 'textfield',
                      maxLength: 255,
                  }
              },
        ]

    });
    comGrid.store.sort([
        { property: 'da_id', direction: 'DESC' }
    ]);
    var forthFrom = Ext.widget('form', {
        id: 'forthFrom',
        plain: true,
        frame: true,
        layout: 'anchor',
        url: '/VipUserGroup/SaveVipUserGroup',
        //  defaults: { anchor: "95%", msgTarget: "side" },
        items: [
              {
                  xtype: 'filefield',
                  fieldLabel: '員工清單',
                  name: 'employee_list',
                  id: 'employee_list',
                  buttonText: '上傳檔案',
                  width: 490,
                  validator:
              function (value) {
                  if (value != '') {
                      var type = value.split('.');
                      var extention = type[type.length - 1].toString().toLowerCase();
                      if (extention == 'xls' || extention == 'xlsx') {
                          return true;
                      }
                      else {
                          return '上傳文件類型不正確！';
                      }
                  }
                  else {
                      return true;
                  }
              },
              },
            {
                xtype: 'displayfield',
                html: "<a href='#' onclick='down()'>點擊下載模板</a>",
                width: 490,

            },
            {
                xtype: 'textfield',
                fieldLabel: '配送名稱',
                id: 'da_title',
                name: 'da_title',
                width: 300,
            },
            {
                xtype: 'combobox',
                fieldLabel: '配送地址',
                store: zipStore2,
                valueField: 'zipcode',
                displayField: 'zipname',
                name: 'addres',
                emptyText: '請選擇...',
                queryModel: 'local',
                editable: false,
                id: 'address',
                width: 300,
            },
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                    {
                        xtype: 'textfield',
                        id: 'company_address',
                        margin: '0 0 0 53',
                        maxLength: 255,
                        width: 328,
                    },
                    {
                        xtype: 'button',
                        id: 'btn_save',
                        text: '新增',
                        handler: onAdd
                    }
                ]
            },
            comGrid,
        ],
        buttonAlign: 'right',
        buttons: [
      {
          text: '保存',
          id: 'saveForth',
          formBind: true,
          disabled: true,
          handler: function () {
              //var length = Ext.getCmp("comGrid").store.data.items;
              //if (length == 0) {
              //    Ext.Msg.alert("提示信息", "公司地址還未填寫");
              //    return;
              //}
              var group_code = Ext.getCmp('group_code').getValue();
              if (group_code == "") {
                  Ext.Msg.alert("提示信息", "公司統編未填寫！");
                  return;
              }
              var tax_id = Ext.getCmp('tax_id').getValue();
              if (tax_id == "") {
                  Ext.Msg.alert("提示信息", "公司代號未填寫！");
                  return;
              }
              if (Ext.getCmp('group_committe_other').getValue().length > 90) {
                  Ext.Msg.alert("提示信息", "福利推廣字數過多！");
                  return;
              }
              if (Ext.getCmp('group_committe_desc').getValue().length > 200)
              {
                  Ext.Msg.alert("提示信息", "推廣簡述字數過多！");
                  return;
              }
              if (Ext.getCmp('check_iden').getValue().c_id==1&&Ext.getCmp('employee_list').getValue() == "") {
                  Ext.Msg.alert("提示信息", "請選擇上傳檔案！");
                  return;
              }
              //var mail = Ext.getCmp('group_committe_mail').getValue();
              //if (mail == "") {
              //    Ext.Msg.alert("提示信息", "公司福委信箱未填寫！");
              //    return;
              //}
              //var regex = /^([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+@([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+\.[a-zA-Z]{2,3}$/;
              //if (!regex.test(mail)) {
              //    Ext.Msg.alert("提示信息", "公司福委信箱格式不正確！");
              //    return;
              //}

              var form = this.up('form').getForm();
              var ageChecked = Ext.getCmp("group_emp_age").items;
              var ageCheck = "";
              for (var i = 0; i < ageChecked.length; i++) {
                  if (ageChecked.get(i).checked) {
                      ageCheck = ageCheck + ageChecked.get(i).inputValue + ",";
                  }
              }
              var benefitChecked = Ext.getCmp("group_benefit_type").items;
              var benefitCheck = "";
              for (var i = 0; i < benefitChecked.length; i++) {
                  if (benefitChecked.get(i).checked) {
                      benefitCheck = benefitCheck + benefitChecked.get(i).inputValue + ",";
                  }
              }
              var promotion = Ext.getCmp("group_committe_promotion").items;
              var promotionCheck = "";
              for (var i = 0; i < promotion.length; i++) {
                  if (promotion.get(i).checked) {
                      promotionCheck = promotionCheck + promotion.get(i).inputValue + ",";
                  }
              }
              var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });

              myMask.show();
              if (form.isValid()) {
                  form.submit({
                      params: {
                          group_id: Ext.getCmp('group_id').getValue(),
                          group_name: Ext.getCmp('group_name').getValue(),
                          eng_name: Ext.getCmp('eng_name').getValue(),
                          tax_id: Ext.getCmp('tax_id').getValue(),
                          group_code: Ext.getCmp('group_code').getValue(),
                          group_capital: Ext.getCmp('group_capital').getValue(),
                          check_iden: Ext.getCmp('check_iden').getValue().c_id,
                          group_emp_number: Ext.getCmp('group_emp_number').getValue(),
                          group_emp_age: ageCheck,
                          group_emp_gender: Ext.getCmp('group_emp_gender').getValue(),
                          group_benefit_type: benefitCheck,
                          group_benefit_desc: Ext.getCmp('group_benefit_desc').getValue(),
                          group_subsidiary: Ext.getCmp('group_subsidiary').getValue().gsu,
                          group_hq_name: Ext.getCmp('group_hq_name').getValue(),
                          group_hq_code: Ext.getCmp('group_hq_code').getValue(),
                          group_committe_name: Ext.getCmp('group_committe_name').getValue(),
                          group_committe_code: Ext.getCmp('group_committe_code').getValue(),
                          group_committe_chairman: Ext.getCmp('group_committe_chairman').getValue(),
                          group_committe_phone: Ext.getCmp('group_committe_phone').getValue(),
                          group_committe_mail: Ext.getCmp('group_committe_mail').getValue(),
                          group_committe_promotion: promotionCheck,
                          group_committe_other: Ext.getCmp('group_committe_other').getValue(),
                          group_committe_desc: Ext.getCmp('group_committe_desc').getValue(),
                          employee_list: Ext.getCmp('employee_list').getValue(),
                      },
                      success: function (form, action) {
                          myMask.show();
                          var result = Ext.decode(action.response.responseText);
                          if (result.success) {
                              myMask.hide();
                              Ext.Msg.alert("提示信息", "保存成功！");
                              VipUserGroupStore.load();
                              editWin.close();
                          }
                          else {
                              myMask.show();
                              Ext.Msg.alert("提示信息", "保存失敗！");
                          }
                      },
                      failure: function () {
                          myMask.hide();
                          Ext.Msg.alert("提示信息", "保存失敗！");
                      }
                  });

              }
          }
      }

        ]
    });



    var allPan = new Ext.panel.Panel({
        width: 550,
        layout: 'card',
        border: 0,
        bodyStyle: 'padding:0px',
        defaults: {
            border: false
        },
        bbar: [
               {
                   id: 'move-prev',
                   text: "上一步",
                   hidden: true,
                   handler: function (btn) {
                       navigate(btn.up("panel"), "prev");
                   }
               },
               '->',
               {
                   id: 'move-next',
                   text: "下一步",
                   formBind: true,
                   margins: '0 5 0 0',
                   handler: function (btn) {
                       navigate(btn.up("panel"), "next");
                   }
               }
        ],
        items: [firstForm, secondForm, forthFrom]
    });

    var editWin = Ext.create("Ext.window.Window", {
        title: "企業用戶管理",
        iconCls: row ? "icon-user-edit" : "icon-user-add",
        width: 560,
        //  y: 100,
        layout: 'fit',
        items: [allPan],
        constrain: true, //束縛
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        closable: false,
        tools: [
{
    type: 'close',
    qtip: CLOSEFORM,
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
                if (row) {
                    var age = row.data.group_emp_age;
                    var ageArr = age.split(',');
                    for (var i = 0; i < ageArr.length; i++) {
                        switch (ageArr[i]) {
                            case "1":
                                Ext.getCmp('20').setValue(true);
                                break;
                            case "2":
                                Ext.getCmp('21_30').setValue(true);
                                break;
                            case "3":
                                Ext.getCmp('31_40').setValue(true);
                                break;
                            case "4":
                                Ext.getCmp('41_50').setValue(true);
                                break;
                            case "5":
                                Ext.getCmp('51_60').setValue(true);
                                break;
                            case "6":
                                Ext.getCmp('61').setValue(true);
                                break;
                        }
                    }
                    var benefitType = row.data.group_benefit_type;
                    var typeArr = benefitType.split(',');
                    for (var i = 0; i < typeArr.length; i++) {
                        switch (typeArr[i]) {
                            case "1":
                                Ext.getCmp('welfare').setValue(true);
                                break;
                            case "2":
                                Ext.getCmp('gift_cash').setValue(true);
                                break;
                            case "3":
                                Ext.getCmp('gift_box').setValue(true);
                                break;
                            case "4":
                                Ext.getCmp('cash').setValue(true);
                                break;
                            case "5":
                                Ext.getCmp('tour').setValue(true);
                                break;
                            case "6":
                                Ext.getCmp('other').setValue(true);
                                break;
                        }
                    }
                    Ext.getCmp('group_id').setValue(row.data.group_id);
                    Ext.getCmp('old_group_code').setValue(row.data.group_code);
                    Ext.getCmp('eng_name').setValue(row.data.eng_name);
                    Ext.getCmp('group_emp_gender').setValue(row.data.group_emp_gender);
                    Ext.getCmp('group_benefit_desc').setValue(row.data.group_benefit_desc);
                    if (row.data.group_subsidiary == 1) {
                        Ext.getCmp('gs1').setValue(true);
                        Ext.getCmp('group_hq_name').setValue(row.data.group_hq_name);
                        Ext.getCmp('group_hq_code').setValue(row.data.group_hq_code);

                    }
                    else if (row.data.group_subsidiary == 0) {
                        Ext.getCmp('gs0').setValue(true);
                        Ext.getCmp('group_hq_name').setValue(row.data.group_hq_name);
                        Ext.getCmp('group_hq_code').setValue("");
                    }
                    if (row.data.check_iden == 1) {
                        Ext.getCmp('yes').setValue(true);
                    }
                    else if (row.data.check_iden == 0) {
                        Ext.getCmp('no').setValue(true);
                    }
                    firstForm.getForm().loadRecord(row);
                    //     secondForm.getForm().loadRecord(row);
                    Ext.getCmp('group_committe_name').setValue(row.data.group_committe_name);



                }
            }
        }
    });
    //   editWin.show();
    if (rowID) {
        var row;
        edit_VipUserGroupStore.load({
            params: {
                relation_id: rowID,
            },
            callback: function () {
                row = edit_VipUserGroupStore.getAt(0);
                editWin.show();
            }
        });
    }
    else {
        editWin.show();
    }

    function onAdd() {
        var address = Ext.getCmp('address').getValue();
        var company_address = Ext.getCmp('company_address').getValue();
        var da_title = Ext.getCmp('da_title').getValue();
        if (address != null || company_address != "") {

            if (address != null && da_title == "") {
                Ext.Msg.alert("提示信息", "請輸入配送名稱！");
            }
            else {
                Ext.Ajax.request({
                    url: '/VipUserGroup/SaveComAddress',
                    method: 'post',
                    params: {
                        user_id: Ext.getCmp('group_id').getValue(),
                        da_title: da_title,
                        da_dist: address,
                        da_address: company_address,
                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            companyAddressStore.load({ params: { user_id: Ext.getCmp('group_id').getValue() } });
                            Ext.getCmp('address').reset();
                            Ext.getCmp('company_address').reset();
                            Ext.getCmp('da_title').reset();
                        }
                        else {
                            Ext.Msg.alert("提示信息", "新增失敗！");
                        }
                    },
                    failure: function () {
                        Ext.Msg.alert("提示信息", "新增失敗！");
                    }
                });
            }




        }
        else {
            Ext.Msg.alert("提示信息", "請選擇或輸入配送地址！");
        }
    }

    function addgcc() {
        var group_id = Ext.getCmp('group_id').getValue();
        var group_committe_chairman = Ext.getCmp('group_committe_chairman').getValue();
        var group_committe_phone = Ext.getCmp('group_committe_phone').getValue();
        var group_committe_mail = Ext.getCmp('group_committe_mail').getValue();
        var regexphone =  /^[-+]?[\d]+$/;
        var regexmail = /^([a-z0-9_\.-]+)@([\da-z\.-]+)\.([a-z\.]{2,6})$/;       
        if (group_committe_chairman == "") {
            Ext.Msg.alert(INFORMATION, "請輸入福委姓名");
        }
        else if (group_committe_phone == "") {
            Ext.Msg.alert(INFORMATION, "請輸入福委電話");
        }
        else if (group_committe_mail == "") {
            Ext.Msg.alert(INFORMATION, "請輸入福委email");
        }   
        else if (!regexphone.test(group_committe_phone)) {
            Ext.Msg.alert(INFORMATION, "福委電話格式錯誤");
        }
        else if (!regexmail.test(group_committe_mail)){
            Ext.Msg.alert(INFORMATION, "福委email格式錯誤");
        }
        else {
            Ext.Ajax.request({
                url: '/VipUserGroup/SaveGCC',
                method: 'post',
                params: {
                    group_id: Ext.getCmp('group_id').getValue(),
                    group_committe_chairman: group_committe_chairman,
                    group_committe_phone: group_committe_phone,
                    group_committe_mail: group_committe_mail,
                },
                success: function (form, action) {
                    var result = Ext.decode(form.responseText);
                    if (result.success) {
                        gccStore.load({ params: { group_id: Ext.getCmp('group_id').getValue() } });
                        Ext.getCmp('group_committe_chairman').reset();
                        Ext.getCmp('group_committe_phone').reset();
                        Ext.getCmp('group_committe_mail').reset();
                    }
                    else {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                    }
                },
                failure: function () {
                    Ext.Msg.alert(INFORMATION, FAILURE);
                }
            });
        }
    }


}
function down() {
    window.open("/VipUserGroup/DownLoad");
}
