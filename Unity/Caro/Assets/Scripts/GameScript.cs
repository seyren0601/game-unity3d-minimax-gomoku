using MiniMax;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem;
using static MiniMax.MiniMax;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameScript : MonoBehaviour
{
    // Thuộc tính để theo dõi lượt đi
    bool playerTurn { get; set; } = true;

    // Thuộc tính để biết khi nào trò chơi bắt đầu
    public static bool GamePlaying {get;set;} = false;

    // Thuộc tính ghi lại trạng thái hiện tại của trò chơi
    State state { get; set; }

    // Các object quân cờ, âm thanh và thông tin khác của trò chơi
    public GameObject Black;
    public GameObject White;
    public AudioSource audioSource;
    public AudioSource sGameOver;
    static Result result {get;set;}
    GameInfo gameInfo;
    Camera main_camera;

    // Update is called once per frame
    void Update()
    {
        // Nếu game đã bắt đầu, khởi tạo các cấu trúc dữ liệu để bắt đầu chạy trò chơi
        if(GamePlaying){
            main_camera = Camera.main;
            gameInfo = gameObject.GetComponent<GameInfo>();
            string[,] startBoard = new string[StartGame.sizeOfBanCo, StartGame.sizeOfBanCo];
            for(int i=0;i<StartGame.sizeOfBanCo;i++){
                for(int j=0;j< StartGame.sizeOfBanCo; j++){
                    startBoard[i, j] = " ";
                }
            }
            state = new State(startBoard, null);
            result = Result.Pending;
            BOARD_SIZE = StartGame.sizeOfBanCo;
            LINE_SIZE = StartGame.lineSize;
            GamePlaying = false;
        }
        //Debug click
        /*Mouse mouse = Mouse.current;
        if(mouse.leftButton.wasPressedThisFrame){
            audioSource.Play();
            Vector3 mousePosition = mouse.position.ReadValue();
            //Debug.Log(mousePosition);
            Ray ray = main_camera.ScreenPointToRay(mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hit)){
                Debug.Log(hit.point);
            }
        }*/

        //GameLoop
        if (result == Result.Pending) {
            // Ở lượt đi của người chơi, 
            // thực hiện kiểm tra vùng click vào và đặt quân đen vào đó
            if (playerTurn) {
                Mouse mouse = Mouse.current;
                if (mouse.leftButton.wasPressedThisFrame) {
                    audioSource.Play();
                    Vector3 mousePosition = mouse.position.ReadValue();
                    //Debug.Log(mousePosition);
                    Ray ray = main_camera.ScreenPointToRay(mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit)) {
                        Vector3 clicked = hit.point;
                        (int, int, Vector3)? move_point = GetCenterPoint(hit.point);
                        if (move_point is not null) {
                            Vector3 center_point = move_point.Value.Item3;
                            int i = move_point.Value.Item1;
                            int j = move_point.Value.Item2;
                            if (state.board[i, j] == " ") {
                                Instantiate(Black, center_point, Quaternion.identity);
                                string[,] newboard = (string[,])state.board.Clone();
                                newboard[i, j] = "X";
                                state = new State(newboard, (state, new MiniMax.Point(i, j), "X"));
                                playerTurn = false;
                            }
                        }
                    }
                }
            }
            // Ở lượt đi của máy, thực hiện chạy MiniMax để tìm nước đi tốt nhất
            // sau đó đặt quân trắng vào đó
            else {
                MiniMax.Point move = MiniMax.MiniMax.AutoPlay_GetMove(state, "O");
                Instantiate(White, gameInfo.center_points[move.x, move.y], Quaternion.identity);
                string[,] newboard = (string[,])state.board.Clone();
                newboard[move.x, move.y] = "O";
                state = new State(newboard, (state, new MiniMax.Point(move.x, move.y), "O"));
                playerTurn = true;
            }
            // Xét kết quả của trạng thái hiện tại
            result = CurrentState(state);
            // Nếu trạng thái hiện tại đã kết thúc (không phải Pending)
            // Chuyển sang màn hình kết thúc và chụp lại trạng thái cuối cùng của bàn cờ
            // và lưu vào folder Assets/Screenshots
            if (result != Result.Pending)
            {
                EndMain.state = state;
                EndMain.whoWin = result.ToString();
                StartCoroutine(TakeAPicture(2f));
                StartCoroutine(WaitASecond(3f));
            }
        }
        
    }

    IEnumerator WaitASecond(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        SceneManager.LoadSceneAsync(2);
    }

    private IEnumerator TakeAPicture(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        string folderPath = "Assets/Screenshots/"; // the path of your project folder

        if (!System.IO.Directory.Exists(folderPath)) // if this path does not exist yet
            System.IO.Directory.CreateDirectory(folderPath);  // it will get created

        var screenshotName =
                                "Screenshot_" +
                                System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + // puts the current time right into the screenshot name
                                ".png"; // put youre favorite data format here
        ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(folderPath, screenshotName), 2); // takes the sceenshot, the "2" is for the scaled resolution, you can put this to 600 but it will take really long to scale the image up
    }

    (int, int, Vector3)? GetCenterPoint(Vector3 Clicked){
        for(int i=0;i<StartGame.sizeOfBanCo;i++){
            for(int j=0;j<StartGame.sizeOfBanCo;j++){
                float x_clicked = Clicked.x;
                float z_clicked = Clicked.z;
                Vector3 render_point = gameInfo.render_points[i,j];
                if(x_clicked >= render_point.x && x_clicked <= render_point.x + gameInfo.square_size
                   && z_clicked >= render_point.z && z_clicked <= render_point.z + gameInfo.square_size)
                {
                    return (i, j, gameInfo.center_points[i,j]);
                }
            }
        }
        return null;
    }


}
