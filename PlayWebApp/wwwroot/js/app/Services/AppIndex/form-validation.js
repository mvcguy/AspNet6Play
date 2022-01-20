//
        // jquery validation
        //
        var $form = $('#frmGreetings');
        var options = {  // options structure passed to jQuery Validate's validate() method
            errorClass: "input-validation-error",
            errorElement: "span",
            errorPlacement: function () {
                onError.apply($form, arguments);
            },
            invalidHandler: function () {
                onErrors.apply($form, arguments);
            },
            messages: {
                txtName: "Please specify your name",
                email: {
                    required: "We need your email address to contact you",
                    email: "Your email address must be in the format of name@domain.com"
                }
            },
            rules: {
                txtName: "required",
                email: {
                    required: true,
                    email: true
                }
            },
            success: function () {
                onSuccess.apply($form, arguments);
            }
        };

        var attachValidation = function (form) {
            form.validate(options);
        };

        var onError = function (error, element) {
            console.log(error);
            $('#txtNameValidation').html(error);
        };

        var onErrors = function (errors) {
            console.log(errors);
        };

        var onSuccess = function (error) {
            console.log(error);
        };

        attachValidation($form);


        //
        // string casing: pascal, camel, pascalJson
        //
        $('#btnConvert').on('click', function () {

            var sampleText = $('#sampleText').val();
            if (!sampleText || sampleText === '') {
                $('#lblOutput').text('Please specify a sample text');
                return;
            }

            var opt = $('input[name="function"]:checked').val();
            if (opt === 'camal') {
                $('#lblOutput').text(sampleText.toCamalCase());
            }
            else if (opt === 'pascal') {
                $('#lblOutput').text(sampleText.toPascalCase());
            }
            else if (opt === 'pascalJson') {
                $('#lblOutput').text(sampleText.toPascalCaseJson());
            }
            else {
                $('#lblOutput').text('Please select a function');
            }

        });

        //
        // draggable table columns
        //
        //$('#colReorder').enableColumnReordering();