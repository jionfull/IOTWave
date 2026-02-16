from PIL import Image, ImageDraw

"""
绿闪纹理生成脚本
独立运行生成绿闪纹理
"""

def create_green_flash_texture():
    """创建绿闪纹理"""
    # 创建16x16的黑色背景图像
    img = Image.new("RGB", (16, 16), color="#000000")
    draw = ImageDraw.Draw(img)
    draw.rectangle([0, 0, 8, 16], fill="#00FF00")
    
    return img

def main():
    """主函数"""
    print("生成绿闪纹理...")
    img = create_green_flash_texture()
    img.save("green_flash.png")
    print("绿闪纹理已生成: green_flash.png")

if __name__ == "__main__":
    main()