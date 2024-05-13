import argparse
from scidownl import scihub_download
import os
import json

def download_paper(paper, paper_type, out):
    scihub_download(paper, paper_type=paper_type, out=out)
    result = (os.path.isfile(out))
    return result

def main():
    parser = argparse.ArgumentParser(description="Download papers from Sci-Hub")
    parser.add_argument("papers", metavar="PAPER", nargs="+", help="List of papers to download")
    parser.add_argument("--paper-type", dest="paper_type", default="pmid", help="Type of paper identifier (default: pmid)")
    parser.add_argument("--out", dest="out", default="./PDF/", help="Output directory (default: ./PDF/)")

    args = parser.parse_args()
    failed_downloads = []
    for paper in args.papers:
        out_path = f"{args.out}/{paper}.pdf"
        if (not download_paper(paper, args.paper_type, out_path)):
            failed_downloads.append(paper)

    with open("failed_ids.txt", "w") as f:
            for failed_id in failed_downloads:
                f.write(f"{failed_id}\n")

if __name__ == "__main__":
    main()
