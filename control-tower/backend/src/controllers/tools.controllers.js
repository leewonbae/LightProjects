
const path = require('path');
const sqlite3 = require('sqlite3');
const Sequelize = require("sequelize");
const fs = require('fs');

exports.compareFileMd5 = async ( req , res, next ) => {
    let sqliteDbFile = req.files[0];
    let dbName = req.body.dbName;
    console.log(dbName);
    const upperPath = path.join(__dirname, '../..'); 
    const resultPath = path.join(upperPath, 'uploadFiles', sqliteDbFile.filename);

    let clientMD5List = await GetSqliteMD5List(resultPath);
    let serverMD5List = await GetServerMD5List(dbName);
    
    let serverMD5Dic = new Map();
   
    // 접두사 제거
    const prefix = "work_";
    for(md5info of serverMD5List )
    {
        let filteredFileName = md5info.file_name.replace(prefix,'');

        if(!serverMD5Dic.has(filteredFileName))
        {
            serverMD5Dic.set(filteredFileName,md5info);
        }
    }

    // client 데이터와 비교
    let resultList = [];
    for(clientInfo of clientMD5List )
    {
        clientInfo.isUsingTableInServer  = false;
        clientInfo.isSameMD5 = false;
        
        if(serverMD5Dic.has(clientInfo.Name))
        {
            clientInfo.isUsingTableInServer = true;
            if(clientInfo.MD5 == serverMD5Dic.get(clientInfo.Name).md5)
            {
                clientInfo.isSameMD5 = true;
            }
        }
                
        resultList.push(clientInfo);
    }

    fs.unlink(resultPath, (err) => {
        if (err) {
            console.log(`${err}`);
          throw (err);
        }
        console.info('파일이 성공적으로 삭제되었습니다.');
    });

    return res.send({rows : resultList});
};

async function GetServerMD5List(dbName) {
    let dataTableName = "data_table";
    if(dbName != 'default')
    {
        dataTableName = `${dbName}_data_table`;
    }

    let dataTableSequelize = new Sequelize(dataTableName,config.DATA_TABLE_DB.username,config.DATA_TABLE_DB.password,
    {
        host: config.DATA_TABLE_DB.host,
        dialect: config.DATA_TABLE_DB.dialect
    });

    try {
        await dataTableSequelize.authenticate();
        console.log(`${dataTableName} DB 연결 성공`);
    }
    catch(error)
    {
        console.log(`${error}`);
    }    
    
    let fileMd5List = await dataTableSequelize.query(
        `SELECT * 
        FROM file_md5`,
        {
            type: dataTableSequelize.QueryTypes.SELECT,
        }
    );
        
    return fileMd5List;
}
async function GetSqliteMD5List(filePath) {
    const db = new sqlite3.Database(filePath, (err) => {
        if (err) {
          console.error('DB 연결 실패:', err.message);
        } else {
          console.log('SQLite DB 연결 성공');
        }
      });
      
      return new Promise(((resolve, reject)=> {
        db.all("SELECT * FROM FileMD5;", [], (err,rows) => {
            if (err) {
              reject(err);
            }
            else
            {
                resolve(rows);
                db.close();
            }
          })
      }))
};
