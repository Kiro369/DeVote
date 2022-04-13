let mySQLite = global.SQLite;

let error404 = {
    "code": "404",
    "status": "Not Found"
}
let error400 = {
    "code": "400",
    "status": "Bad Request"
}

function notFoundErrorHandler(req, res, resourceType, paramName, paramValue) {
    let errors = []
    let error = error404;
    error["detail"] = `No ${resourceType} was found for ${paramName} : ${paramValue}`;
    errors.push(error)
    res.status(404).send({ errors });
}

// A higer order function to wrap a given function and enable it to handle and throw erros.
function handleErrors(fn, params) {
    fn = fn.bind(mySQLite)

    return function () {
        return fn(params)
            .catch((error) => {
                console.error("Opps")
                throw error
            })
    }
}

// A higer order function to execute a given function and enable it to catch and log erros.
async function catchErrors(fn, req, res) {
    let isErrorCatched = false;
    let errors = []
    await fn()
        .catch(err => {
            console.error("Error Catched"); console.error(err)
            console.error(err.errno); console.error(err.message)

            const errorNo = err.errno;
            if (errorNo == 19) {
                let error = error400;
                error["detail"] = `Your request parameters values are invalid`
                errors.push(error)
                res.status(400).send({ request: req.query, errors })
            }
            isErrorCatched = true;
        });

    return isErrorCatched;
}

module.exports = { notFoundErrorHandler, handleErrors, catchErrors };